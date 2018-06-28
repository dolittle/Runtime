# Events Relativity

## Protobuf

We're using [gRPC](https://www.grpc.io) for communication which relies on protobuf.
All services exposed are exposed through `.proto` files. After changing these you need to
generate the source by running the following in your shell:

```shell
$ ./generate_protos.sh
```

## Vocabulary

The vocabulary in this is loosely based on [general relativity](https://en.wikipedia.org/wiki/General_relativity) and a tribute to the late professor [Stephen Hawking](https://en.wikipedia.org/wiki/Stephen_Hawkings) with a semi scientific approach linking the concepts
to what they actually do in the software.

### [Event Horizon](https://en.wikipedia.org/wiki/Event_horizon)

The event horizon represents the final entry for committed events. At this point they can only be seen by other singularities.
In Dolittle, a singularity would then be a running node that is connected to this event horizon and receives a stream of particles.
The particles, representing committed events. By committed events, we mean events that has been persisted into an event store.

### [Singularity](https://en.wikipedia.org/wiki/Gravitational_singularity)

A singularity represents a single destination point for an event-particle. An event horizon can [spaghettified](https://en.wikipedia.org/wiki/Spaghettification) particles into multiple singularities.

### [Quantum Tunnel](https://en.wikipedia.org/wiki/Quantum_tunnelling)

Each singularity can connect any event horizon, they establish a quantum tunnel for the purpose of moving particles across.

### [Wave Function](https://en.wikipedia.org/wiki/Wave_function)

Part of the process moving through a quantum tunnel means at times the [quantum state](https://en.wikipedia.org/wiki/Quantum_state) gets collapsed.
The state, being an event particle has the possibility to change between different versions of the the software.
This process is described sa [wave function collapse](https://en.wikipedia.org/wiki/Wave_function_collapse).
A undefined process in Dolittle, but seems interesting is the [wave function renormalization](https://en.wikipedia.org/wiki/Wave_function_renormalization).

### [Particle](https://en.wikipedia.org/wiki/Particle)

Particles are small objects, and in Dolittle there is an event particle. This is the thing that passes through the event
horizon into each singularity.