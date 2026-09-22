# Does Loony transfer to Partas.Signals' synchronization design?

> **Question.** The author of this library also wrote [nim-works/loony](https://github.com/nim-works/loony), a
> lock-free MPMC FIFO queue in Nim. Does any of that work — the implementation, or the ideas — transfer to the
> cross-thread boundary that Partas.Signals (Project Ranvier) needs on native .NET?
>
> **Short answer.** No, not the implementation, and only one idea — and that idea is ordinary reference counting
> wearing a lock-free costume. The graph boundary is an MPSC inbox carrying roughly one message per settle or
> external write, feeding a single serial consumer. Loony is an unbounded MPMC queue whose headline contribution
> is deterministic node reclamation *without a garbage collector*, tuned for hundreds of contending threads. On
> .NET the GC gives that contribution away for free, `ConcurrentQueue<T>` already is a segmented lock-free MPMC
> queue, and the boundary's actual bottleneck is the serial consumer, not the queue. Use
> `System.Threading.Channels` (or a `ConcurrentQueue` + one `Interlocked` flag), measure it against
> `MailboxProcessor.Post`, and close §D.1 of the suspension research with a number.

Conventions: ⚠️ marks a claim I could not verify from source and am labelling speculation. Everything else is
cited to a file, line, or URL.

---

## §1. What Loony is

### 1.1 Provenance

Loony is a pure-Nim implementation of the queue in **O. Giersch and J. Nolte, "Fast and Portable Concurrent FIFO
Queues With Deterministic Memory Reclamation", IEEE TPDS 33(3):604–616, March 2022,
doi:10.1109/TPDS.2021.3097901** ([IEEE Xplore](https://ieeexplore.ieee.org/document/9490347/)). The paper's
algorithm is known as **looqueue**; the author ships a C++ reference ([oliver-giersch/looqueue](https://github.com/oliver-giersch/looqueue))
and a Rust port ([oliver-giersch/looqueue-rs](https://github.com/oliver-giersch/looqueue-rs)). Loony's README credits
Giersch for proposing the algorithm and reviewing the Nim implementation, and records that it was purpose-built for
[nim-works/cps](https://github.com/nim-works/cps) continuation scheduling, then generalised by disruptek to carry any
`ref object` ([README](https://github.com/nim-works/loony/blob/main/README.md)). A copy of the paper is vendored at
[`papers/GierschEtAl.pdf`](https://github.com/nim-works/loony/blob/main/papers/GierschEtAl.pdf).

Note for the record: it is **not** Nikolaev's SCQ ("A Scalable, Portable, and Memory-Efficient Lock-Free FIFO
Queue", DISC 2019, [arXiv:1908.04511](https://arxiv.org/abs/1908.04511)). Both belong to the post-LCRQ family of
fetch-and-add array queues; they differ chiefly in reclamation — SCQ is bounded and needs none, looqueue is
unbounded and integrates its own.

### 1.2 The algorithm, precisely

Sources: [`loony.nim`](https://github.com/nim-works/loony/blob/main/loony.nim),
[`loony/node.nim`](https://github.com/nim-works/loony/blob/main/loony/node.nim),
[`loony/spec.nim`](https://github.com/nim-works/loony/blob/main/loony/spec.nim), and the upstream pseudocode in
[`looqueue/ALGORITHMS.md`](https://github.com/oliver-giersch/looqueue/blob/master/ALGORITHMS.md).

**Structure.** A singly linked list of nodes. Each node is `slots: array[N, Atomic[uint]]` (default
`loonySlotCount = 1024`), `next: Atomic[NodePtr]`, and a `ctrl: ControlBlock` for reclamation. The queue object
is three cache-line-padded atomics: `head`, `tail` (both `TagPtr`) and `currTail` (`loony.nim`, `LoonyQueueImpl`,
`{.align: 128.}`).

**Tagged pointer.** `TagPtr = uint`: the node pointer and the slot index share one machine word. Nodes are
allocated with `allocAligned0(sizeof(Node), NODEALIGN)` where `NODEALIGN = 1 shl loonyNodeAlignment` (default 11
bits), so the low 11 bits of every node address are zero and hold the index (`spec.nim`: `TAGBITS`, `NODEALIGN`,
`TAGMASK`, `PTRMASK`). This is what lets a single `fetchAdd(1)` on the tail word both reserve a slot and carry the
node identity atomically, with no double-width CAS.

**Fast path (≈ "99% of the time", per the comment in `loony.nim`).**

- *Push:* `tag = tail.fetchAdd(1)`; if `tag.idx < N`, `prev = slots[idx].fetchAdd(ptr or WRITER)`. If `prev` is
  `0` or `RESUME`, done. If `prev` already has `READER`, a consumer arrived first — the slot is a tombstone,
  the producer abandons it and retries (`pushImpl`).
- *Pop:* sample `tail` then `head`; if `isEmptyImpl` return nil/default; else `head.fetchAdd(1)`; if `idx < N`,
  `prev = slots[idx].fetchAdd(READER)`. If `prev` has `WRITER`, mask off the flag bits and return the element.
  Otherwise the consumer's `READER` mark stays as a tombstone and the consumer retries (`popImpl`).

Slot state bits (`spec.nim`): `RESUME = 0b001`, `WRITER = 0b010`, `READER = 0b100`, `CONSUMED = READER or WRITER`,
`SLOTMASK = not (RESUME or WRITER or READER)`. The three low bits are free because the stored element pointers are
themselves aligned.

**Slow path.** When the reserved index is `≥ N`, `advTail` / `advHead` run. `advTail` is documented as a
"modified version of the Michael-Scott algorithm": allocate a node with the element pre-stored in slot 0, CAS it
onto `origTail.next`, then CAS-loop the global tail to `[node, 1]`. Losers deallocate their speculative node and
help swing the tail. `advHead` CASes head to `[next, 0]` or reports `QueueEmpty`.

**Progress guarantees.** Lock-free, linearizable; **not** wait-free. [`looqueue/PROOF.md`](https://github.com/oliver-giersch/looqueue/blob/master/PROOF.md)
claims "full non-blocking and lock-free progress guarantees" and gives linearization points for both operations.
The fast-path FAA never spins, but the slow-path CAS loops are unbounded under contention, and the fast path can
livelock transiently ("at most for N steps") when producers and consumers keep abandoning each other's slots.
Loony's README claims only "lock-free". No producer-side wait-freedom is claimed anywhere.

**Thread bounds.** Because the index shares the word with the pointer, over-reservation past `N` must not spill
into pointer bits. `PROOF.md` bounds this: with `B` tag bits and `N` slots, producers `P ≤ 2^B − N − 1` and
consumers `C ≤ (2^B − N − 1)/2`. For the defaults (B = 11, N = 1024) that is ~1023 producers / ~511 consumers;
Loony's README states 1,025 / 512 at 11 bits and 64,610 / 32,255 at `-d:loonyNodeAlignment=16` (the small
off-by-one/two differences between README and proof are not material). These bounds are on *all threads that ever
touch the queue*, not concurrently active ones.

### 1.3 Memory reclamation — the actual contribution

This is what the paper's title is about and what distinguishes looqueue from LCRQ/SCQ. There are **no hazard
pointers and no epochs**. Each node's `ControlBlock` (`spec.nim`) holds:

- `headMask`, `tailMask : Atomic[uint32]` — low 16 bits = running count of slow-path operations that have finished
  with this node; high 16 bits = the *final* count, i.e. the total number of slow-path operations that will ever
  touch it;
- `reclaim : Atomic[uint8]` — three milestone bits `SLOT = 0b001`, `DEQ = 0b010`, `ENQ = 0b100`.

The thread that actually swings `tail` (or `head`) off a node knows the overshoot `curr.idx − N`, which is exactly
the number of threads that over-reserved into the slow path; it writes that as the final count
(`incrEnqCount(node, curr.idx − N)` → `fetchAddTail((final shl 16) + 1)`). Every straggler adds 1 and compares the
running count with whatever final count is present. Whoever observes `running == final` is the last one out and
sets `ENQ` (resp. `DEQ`) in `reclaim`. Separately, `tryReclaim(node, start)` walks the slots from `start`, sets
`RESUME` on any slot not yet `CONSUMED` (handing the obligation to whichever thread completes that slot later), and
if it reaches `N` sets `SLOT`. **A node is freed by whichever operation sets the third bit** — the code at each of
the three sites tests the *prior* value against the other two bits (`node.nim`: `tryReclaim`, `incrEnqCount`,
`incrDeqCount`).

In one sentence: it is participant-count reference counting, done with FAA on packed counters, where the node
itself records how many participants exist. That is elegant and it is exactly why the paper matters *in an
unmanaged runtime*. Nim under ARC has no tracing GC; a lock-free node cannot be freed until every thread that
might still dereference it has left, and this scheme answers "who was last?" without a global epoch or per-thread
announcement arrays.

### 1.4 Runtime-specific baggage worth naming

- **Element handoff is refcount surgery.** `prepareElement` casts the `ref` to `uint`, ORs in `WRITER`, and
  `wasMoved`s the source so ARC does not decrement; `popImpl` compensates with `atomicDecRef` (`node.nim`,
  `loony.nim`). Elements must be `ref`/`ptr` (empty pop returns `nil`). ORC is unsupported (README, issue #4).
- **Explicit fences.** `push`/`pop` issue `atomicThreadFence(ATOMIC_RELEASE/ACQUIRE)` around the slot FAA, with
  `unsafePush`/`unsafePop` variants that skip them. The README calls this "by far the most costly primitive".
- **Cache-line rotation.** With `loonyRotate` the slot index is permuted so consecutive reservations land on
  different cache lines (`prn` in `node.nim`); this is a contention optimisation.
- **Ward** (`loony/ward.nim`) is a thin state-manager wrapper: compile-time flags `PushPausable`, `PopPausable`,
  `Clearable`, `PoolWaiter`, with `pause`/`resume` as atomic OR/AND on a `uint16`, a futex-based `wait`/`wake`
  for pool threads, `killWaiters` (pause pops + `wakeAll`), and an "UNSTABLE/UNTESTED" `clear`. The README
  strikes it through as unlikely to remain in the library.

---

## §2. What the graph actually needs from synchronization

### 2.1 The consistency model, from the scaffolded code

`Partas.Signals/src/Partas.Signals/Types.fs` fixes the model: one graph is one consistency domain, and the only
cross-thread seam is `IGraphDispatcher`:

```fsharp
/// Where settle continuations are marshalled back to.
/// ... This seam is where a dispatcher, synchronisation context, or channel lives — at the graph
/// boundary, never at node level. A per-node actor measured 4.9 µs per round trip on .NET 10
/// (9.6 µs on .NET 11 RC), slower than throwing ...
type IGraphDispatcher =
    abstract Post: (unit -> unit) -> unit
```

`Core.fs` confirms there is no synchronization anywhere inside the graph: `Graph.current` is a plain mutable,
`Signal<'T>.Value` setter iterates `Seq.toArray observers` and calls `MarkDirty` directly, `AsyncSource.Settle`
does the same. Node-to-node propagation is direct calls, as
[RESEARCH-suspension-mechanism.md §3.3](RESEARCH-suspension-mechanism.md) mandates ("Node-to-node propagation
stays direct calls"). This mirrors upstream, where every global is unguarded because JS is single-threaded
([RESEARCH-async-reactive-graph-dotnet.md §3.1](RESEARCH-async-reactive-graph-dotnet.md)).

### 2.2 What crosses the boundary, and how often

Three kinds of message, all of them "run this closure on the graph's turn":

1. **Settle continuations.** A `Task` completes on a pool thread; the value (or error) must be written into the
   `AsyncSource` and dependents marked. Upstream this is the `.then(v => asyncWrite(v) ...)` at
   [`async.ts:631-648`](../solid/packages/signals/src/core/async.ts#L631), which lands in `notifyStatus` and
   schedules one `flush` via the `scheduled` latch at
   [`scheduler.ts:238-240`](../solid/packages/signals/src/core/scheduler.ts#L238):
   `if (scheduled) return; scheduled = true; ... queueMicrotask(flush)`. One settle → one marshal; multiple settles
   in one turn → still one flush.
2. **External writes** from a thread that is not the graph's thread (`Signal.Value <- v` from a worker).
3. **Disposal / cancellation** of an owner scope whose pending children hold in-flight `Task`s
   ([`async.ts:138`](../solid/packages/signals/src/core/async.ts#L138),
   [`:459-466`](../solid/packages/signals/src/core/async.ts#L459)).

The regime is stated in the suspension research and I found nothing to contradict it:
[§3.3](RESEARCH-suspension-mechanism.md) — "At one message per *flush cycle* or per *cross-thread marshal* — not
per node". `MailboxProcessor.Post` measured 71.7 ns / 49 B on .NET 10 and 138.2 ns on .NET 11 RC (§3.1 table). The
graph's own work per flush is the serial settle walk, which for any non-trivial graph is thousands of nanoseconds.
**The queue is not on the critical path; the consumer is.**

### 2.3 The shape, stated as a queue problem

| Property | Graph boundary | Loony |
| --- | --- | --- |
| Producers | many (pool threads, workers) | many |
| Consumers | **exactly one** — the graph turn | many |
| Ordering | FIFO desirable, not load-bearing (flush recomputes to a fixed point regardless) | FIFO, linearizable |
| Throughput regime | ~1 message per settle, bursts of maybe tens | designed for saturating contention across hundreds of threads |
| Bounded? | unbounded is fine; backpressure would *break* settle (a pool thread must never block on the graph) | unbounded, no backpressure |
| Consumer wake-up | **required** — the graph thread/island must be *told* there is work | not provided; `Ward.PoolWaiter` bolts on a futex |
| Element type | closures / small message DU | `ref` only |
| Reclamation | GC | deterministic, participant-counted |

So the boundary is an **MPSC inbox with a wake-up**, not an MPMC queue. The MPMC generality Loony pays for is
unused, and the one thing the boundary needs that a bare queue does not provide — waking the single consumer —
is the one thing Loony does not have in its core.

### 2.4 What .NET already ships for this exact shape

From the runtime sources (fetched from `dotnet/runtime` `main`):

- **`ConcurrentQueue<T>`** — "a linked list of bounded ring buffers, each of which has a head and a tail index"
  ([`ConcurrentQueue.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Concurrent/ConcurrentQueue.cs)),
  `InitialSegmentLength = 32`, `MaxSegmentLength = 1024 * 1024`, doubling. Each segment
  ([`ConcurrentQueueSegment.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Concurrent/ConcurrentQueueSegment.cs))
  is Vyukov's bounded MPMC ring: per-slot `SequenceNumber`, producers `CompareExchange` on `Tail`, consumers on
  `Head`, `PaddedHeadAndTail` on separate cache lines. The only lock is `_crossSegmentLock`, taken in
  `EnqueueSlow`/`TryDequeueSlow` when a segment must be added or retired. Retired segments are simply dropped for
  the GC.
- **`Channel.CreateUnbounded<T>(SingleReader = true)`** →
  [`SingleConsumerUnboundedChannel<T>`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Threading.Channels/src/System/Threading/Channels/SingleConsumerUnboundedChannel.cs):
  backing store is `SingleProducerSingleConsumerQueue<T>`, so **`TryWrite` takes `lock (SyncObj)` on every
  call** to serialise the writers; `TryRead` is lock-free; a parked `ReadAsync` is completed directly from the
  writer (outside the lock). The multi-reader
  [`UnboundedChannel<T>`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Threading.Channels/src/System/Threading/Channels/UnboundedChannel.cs)
  uses a `ConcurrentQueue<T>` and *also* locks in `TryWrite` (to check `_doneWriting` and hand off to a blocked
  reader); `TryRead` is lock-free.
- **`MailboxProcessor`** ([`mailbox.fs`](https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/mailbox.fs)):
  an `arrivals: Queue` under `lock syncRoot`, single reader, woken by a saved continuation (fast path) or an
  `AutoResetEvent` (timeout/cancellation path). Structurally the same as `SingleConsumerUnboundedChannel`, with
  more allocation per message (the 49 B/op measured in §3.1 of the suspension research).

**Observation that reframes §D.1.** Both Channels variants take a monitor on *every write*. The suspension
research's preference for Channels over `MailboxProcessor` was based on allocation profile, not on lock-freedom —
and that is the right basis, because neither is lock-free on the producer side. If lock-free writes matter, the
primitive that has them is `ConcurrentQueue<T>` (fast path is one CAS, no monitor), plus a separate wake-up.

---

## §3. Does Loony transfer? — Verdict

### 3.1 The implementation: no

Four independent reasons, any one of which would be sufficient.

1. **Its headline feature is void on a GC runtime.** Deterministic node reclamation exists to answer "may I free
   this node?" without a tracing collector. On .NET, a retired `ConcurrentQueue` segment is just unreachable and
   the GC collects it; there is no ABA on managed references (the runtime never reuses an address while a
   reference to it is live), so the entire `ControlBlock` / `RESUME` / final-count machinery — the part of
   looqueue that is actually novel — solves a problem .NET does not have.
2. **The tagged pointer is not expressible on managed references.** Loony packs the slot index into the low
   bits of an aligned node address and FAAs the whole word. The CLR does not expose object addresses to managed
   code, the GC moves objects, and `Interlocked.Add` on a `nint` derived from a pinned object is possible but
   would require pinning every node for its lifetime (or an unmanaged arena) and `unsafe` code throughout.
   ⚠️ It could be done with `NativeMemory.AlignedAlloc` and unmanaged node structs, but at that point one is
   writing a native queue in F# to hold managed closures, which need a `GCHandle` per element — the exact
   allocation you were trying to avoid.
3. **The regime is wrong by three or four orders of magnitude.** looqueue's FAA-vs-CAS advantage over
   Michael-Scott / Vyukov-style queues appears when many threads contend on the same word so that CAS retry loops
   thrash. The graph boundary sees roughly one message per settle, produced by whichever pool thread finished a
   `Task`, consumed by one serial turn. At that rate `ConcurrentQueue.Enqueue`'s single CAS succeeds first try.
   ⚠️ I have not measured `ConcurrentQueue.Enqueue` in this harness; the claim is that uncontended CAS and
   uncontended FAA cost the same instruction class, and the numbers to settle it are the §4.3 benchmark.
4. **It is MPMC; the boundary is MPSC with a wake-up.** The graph has one consumer. Loony's core provides no
   consumer notification at all — that was bolted on in `Ward.PoolWaiter` via futex and marked experimental. The
   thing the boundary genuinely needs (park the graph turn, wake it on first message, coalesce a burst into one
   flush) is exactly what `Channel.ReadAsync` / `WaitToReadAsync`, or a `scheduled` latch + dispatcher `Post`,
   already give.

Secondary reasons: Loony's thread-count bounds, cache-line slot rotation, explicit acquire/release fences, and
`ref`-only elements are all Nim/contention-specific baggage with no counterpart in the design here.

### 3.2 The ideas: mostly no, with one small yes

| Loony idea | Transfers? | Why / why not |
| --- | --- | --- |
| FAA slot reservation on a tagged pointer | No | Needs address arithmetic on nodes (§3.1 reason 2); `ConcurrentQueue` gets equivalent uncontended cost with CAS. |
| READER/WRITER tombstone, abandon-and-retry | No | Only meaningful when consumers can outrun producers on the *same slot*; the graph's single consumer never races itself. |
| Three-milestone participant-counted reclamation | **Yes, as a pattern — not as a queue** | See below. |
| Final-count-in-high-bits packed counter | No | GC-managed lifetime removes the need; where a count is needed a plain `Interlocked.Decrement` on an `int` suffices. |
| Cache-line rotation of slot indices | No | Contention optimisation; irrelevant at one message per flush. |
| Bounded memory under backpressure | No | Loony is unbounded and has no backpressure; nothing to transfer. The graph must not apply backpressure to settlers anyway (§2.3). |
| `Ward` pause/resume/killWaiters | No, already exists | `ChannelWriter.TryComplete` / `Complete` plus a `CancellationToken` in the dispatcher is the same shape. |

**The one transferable pattern.** Loony frees a node when the *last of three independent participants* departs,
and it lets any of them be the one that does the freeing. The graph has the same problem in a different coat:
an `AsyncSource` (or async `Memo`) can (a) lose its last observer while (b) a `Task` still holds a strong
reference to it and will later call `Settle` (the upstream cases at
[`async.ts:138`](../solid/packages/signals/src/core/async.ts#L138) and
[`:459-466`](../solid/packages/signals/src/core/async.ts#L459), flagged in
[RESEARCH-async-reactive-graph-dotnet.md §3.1](RESEARCH-async-reactive-graph-dotnet.md) as non-negotiable
deterministic teardown). The correct rule is Loony's rule: teardown runs when *both* "unobserved/disposed" and
"flight completed or cancelled" have been recorded, and whichever of the two arrives second performs it. In .NET
that is a two-bit `Status` flag set under the graph's turn (no atomics needed, because both transitions are
marshalled onto the same consistency domain) — or, if you ever let `Settle` be called off-turn, a single
`Interlocked.Or` on an `int`. It is worth writing into `FlightPolicy`/disposal code with a comment naming the
invariant; it is not worth a queue.

⚠️ Speculation, clearly labelled: if Partas.Signals ever grows a *multi-graph* scheduler — many islands, each a
consistency domain, with pool threads pulling "next graph to flush" from a shared work queue — that queue is
genuinely MPMC and moderately contended, and looqueue-class FAA queues are the right literature. Even then the
baseline to beat is `ConcurrentQueue<T>`, and `ThreadPool.UnsafeQueueUserWorkItem` per island (which uses the
pool's own work-stealing queues) would likely make the shared queue unnecessary. That scenario is explicitly out
of scope today ([RESEARCH-async-reactive-graph-dotnet.md §3.1](RESEARCH-async-reactive-graph-dotnet.md), option 4:
"Lock-free concurrent graph — No prior art attempts it. Out of scope.").

### 3.3 Verdict, bluntly

**Loony does not transfer. Do not port it, do not take it as a benchmark target, and do not let it pull the
boundary design toward MPMC.** The boundary is a one-consumer inbox whose cost is dominated by the serial flush it
feeds. Sophisticated lock-free MPMC machinery here would be over-engineering by construction, and on .NET its
distinctive component (GC-free reclamation) is dead weight. The single idea worth keeping — "last participant out
tears down" for pending nodes — is refcounting and needs no queue.

---

## §4. What to do about it

### 4.1 Dispatcher implementations to write (native)

Two, both behind the existing `IGraphDispatcher`, chosen at construction per `GraphOptions`:

**(a) UI / dispatcher-affine.** `SynchronizationContextDispatcher(ctx: SynchronizationContext)`:
`Post work = ctx.Post((fun _ -> work ()), null)`. Serialisation and wake-up are the host's problem (WPF/Avalonia
`Dispatcher`, Blazor). No queue of our own at all. Coalescing: keep upstream's `scheduled` latch
([`scheduler.ts:238-240`](../solid/packages/signals/src/core/scheduler.ts#L238)) so N settles in one host turn
produce one flush.

**(b) Server / headless island.** The minimal correct shape, which is also what `SignalIsland<T>` and Solid's
`scheduled` latch are:

```fsharp
type IslandDispatcher() =
    let inbox = ConcurrentQueue<unit -> unit>()      // lock-free MPMC; we use it MPSC
    let mutable scheduled = 0                        // 0 = idle, 1 = a pump is queued/running
    let rec pump (_: obj) =
        let mutable work = Unchecked.defaultof<_>
        while inbox.TryDequeue(&work) do work ()
        Volatile.Write(&scheduled, 0)
        // re-check: a producer may have enqueued between the last TryDequeue and the reset
        if not inbox.IsEmpty && Interlocked.CompareExchange(&scheduled, 1, 0) = 0 then
            ThreadPool.UnsafeQueueUserWorkItem(pump, null) |> ignore
    interface IGraphDispatcher with
        member _.Post work =
            inbox.Enqueue work
            if Interlocked.CompareExchange(&scheduled, 1, 0) = 0 then
                ThreadPool.UnsafeQueueUserWorkItem(pump, null) |> ignore
```

Properties: producer path is one `ConcurrentQueue.Enqueue` (single CAS, no monitor) plus one `CompareExchange`;
the only time a pool work item is queued is the first message of a burst; the graph's turn is serialised by the
`scheduled` latch, never by a lock; bursts coalesce into one pump. This is strictly less machinery than either
`MailboxProcessor` or Channels, and it is the exact .NET spelling of upstream's `scheduled = true; queueMicrotask(flush)`.

**(c) Channels variant**, for comparison and as the fallback if (b) shows a bug:
`Channel.CreateUnbounded<unit -> unit>(UnboundedChannelOptions(SingleReader = true, AllowSynchronousContinuations = false))`,
one long-running consumer `task { while! reader.WaitToReadAsync() do while reader.TryRead(&w) do w () }`.
Costs a monitor per `TryWrite` (§2.4) but gives `Complete`/cancellation for free.

Deterministic teardown for either: `Post` after `Complete` must be a defined no-op or throw — pick one and test
it — and disposal of a graph must drain or cancel in-flight settles (the §3.2 two-milestone rule).

### 4.2 Fable side

`fable-library-js` ships `MailboxProcessor.js` but not `System.Threading.Channels` or `ConcurrentQueue`
([RESEARCH-suspension-mechanism.md §3.3, Fable note](RESEARCH-suspension-mechanism.md)). The JS dispatcher is
simply `ImmediateDispatcher` (already in `Types.fs`) plus the `scheduled` latch and `queueMicrotask` — no queue
type needed at all, because there is one thread. This is the construction-time-policy split
[RESEARCH-async-reactive-graph-dotnet.md §4.2](RESEARCH-async-reactive-graph-dotnet.md) already prescribes; no
`#if FABLE_COMPILER` in core.

### 4.3 The measurement that closes §D.1

[RESEARCH-suspension-mechanism.md §D.1](RESEARCH-suspension-mechanism.md) leaves "whether Channels beats
MailboxProcessor at the boundary" unmeasured. Extend the existing harness (`scratchpad/reactive-spike/ThrowCost.fs`,
same rig: Ryzen 9 9900X, Release, no debugger, 200-iteration warmup, forced GC per row) with four rows, each
measuring **producer-side cost of one `Post` from a pool thread and end-to-end latency to the consumer running the
closure**, at 1 producer and at 8 producers:

| Row | Primitive |
| --- | --- |
| 1 | `MailboxProcessor.Post` (existing 71.7 ns / 138.2 ns figure, re-run as control) |
| 2 | `Channel.CreateUnbounded(SingleReader = true)` `TryWrite` |
| 3 | `ConcurrentQueue.Enqueue` + `Interlocked` latch + `UnsafeQueueUserWorkItem` (§4.1 b) |
| 4 | `SynchronizationContext.Post` on a trivial single-thread context (§4.1 a control) |

Report ns/op and B/op on .NET 10 and .NET 11 RC. ⚠️ Prediction, to be falsified: row 3 < row 2 < row 1 on both,
with all three under 200 ns and all three irrelevant next to a flush of ≥ 100 nodes. If row 3 is not the cheapest
at 8 producers, that is the one result that would justify reading the FAA-queue literature again — and the
correct next step would still be `ConcurrentQueue`'s own segment tuning, not a port.

### 4.4 Things not to do

- Do not add a lock-free MPMC queue dependency or implementation to Partas.Signals.
- Do not make the boundary bounded or apply backpressure to settlers; a `Task` continuation must never block on
  the graph.
- Do not lower `IGraphDispatcher` below the graph boundary. The per-node actor was measured at 4.9 µs / 9.6 µs
  per round trip ([§3.1](RESEARCH-suspension-mechanism.md)); the verdict there stands and nothing in Loony changes
  it.

---

## §5. Sources

**Loony / looqueue**

- [nim-works/loony](https://github.com/nim-works/loony) — [`README.md`](https://github.com/nim-works/loony/blob/main/README.md),
  [`loony.nim`](https://github.com/nim-works/loony/blob/main/loony.nim),
  [`loony/node.nim`](https://github.com/nim-works/loony/blob/main/loony/node.nim),
  [`loony/spec.nim`](https://github.com/nim-works/loony/blob/main/loony/spec.nim),
  [`loony/ward.nim`](https://github.com/nim-works/loony/blob/main/loony/ward.nim),
  [`papers/GierschEtAl.pdf`](https://github.com/nim-works/loony/blob/main/papers/GierschEtAl.pdf),
  [`benchmarks/`](https://github.com/nim-works/loony/tree/main/benchmarks) (bench_{mpmc,mpsc,spmc,spsc}.nim; no
  external comparison targets visible in the listing).
- O. Giersch, J. Nolte, "Fast and Portable Concurrent FIFO Queues With Deterministic Memory Reclamation", IEEE
  TPDS 33(3):604–616, 2022, doi:10.1109/TPDS.2021.3097901 — [IEEE Xplore](https://ieeexplore.ieee.org/document/9490347/).
- [oliver-giersch/looqueue](https://github.com/oliver-giersch/looqueue) —
  [`ALGORITHMS.md`](https://github.com/oliver-giersch/looqueue/blob/master/ALGORITHMS.md),
  [`PROOF.md`](https://github.com/oliver-giersch/looqueue/blob/master/PROOF.md);
  [oliver-giersch/looqueue-rs](https://github.com/oliver-giersch/looqueue-rs).
- R. Nikolaev, "A Scalable, Portable, and Memory-Efficient Lock-Free FIFO Queue", DISC 2019 —
  [arXiv:1908.04511](https://arxiv.org/abs/1908.04511) (cited only to distinguish it from looqueue).

**.NET primitives** (dotnet/runtime and dotnet/fsharp, `main`)

- [`ConcurrentQueue.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Concurrent/ConcurrentQueue.cs),
  [`ConcurrentQueueSegment.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Concurrent/ConcurrentQueueSegment.cs)
  (credits [1024cores bounded MPMC queue](http://www.1024cores.net/home/lock-free-algorithms/queues/bounded-mpmc-queue)).
- [`UnboundedChannel.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Threading.Channels/src/System/Threading/Channels/UnboundedChannel.cs),
  [`SingleConsumerUnboundedChannel.cs`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Threading.Channels/src/System/Threading/Channels/SingleConsumerUnboundedChannel.cs).
- [`FSharp.Core/mailbox.fs`](https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/mailbox.fs).

**This repository**

- [`docs/RESEARCH-suspension-mechanism.md`](RESEARCH-suspension-mechanism.md) §3.1 (measurements), §3.3 (boundary
  recommendation), §D.1 (open question).
- [`docs/RESEARCH-async-reactive-graph-dotnet.md`](RESEARCH-async-reactive-graph-dotnet.md) §3.1 (threading
  options table), §4.2 (construction-time policy).
- `C:\Users\shaya\RiderProjects\Partas.Signals\src\Partas.Signals\Types.fs` (`IGraphDispatcher`,
  `ImmediateDispatcher`, `GraphOptions`), `Core.fs` (`Graph`, `Signal`, `AsyncSource`, `Memo`).
- Upstream: [`solid/packages/signals/src/core/scheduler.ts:238-240`](../solid/packages/signals/src/core/scheduler.ts#L238),
  [`async.ts:631-648`](../solid/packages/signals/src/core/async.ts#L631),
  [`async.ts:138`](../solid/packages/signals/src/core/async.ts#L138),
  [`async.ts:459-466`](../solid/packages/signals/src/core/async.ts#L459).
