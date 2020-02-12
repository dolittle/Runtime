# Single server MongoDB replica set
This directory contains scripts necessary to build a MongoDB Docker image that starts as a single server replica set.
It builds on the [officially maintained Docker MongoDB image](https://hub.docker.com/_/mongo), and adds a startup script and some default command line arguments to make the MongoDB server initiate a fresh replica set on first startup.

## Running locally
To start a new MongoDB server so that it is available on `localhost:27017`, run the following command in your favorite shell:
```bash
$ docker run -d -p 27017:27017 dolittle/mongo
4a05adadc29a672c695a57cbd5fb87c120d1201eda5a7d6f4bc7c68c31c5dffd
```

Then verify that the container is running with:
```bash
$ docker ps
CONTAINER ID        IMAGE                  COMMAND                  CREATED             STATUS              PORTS                      NAMES
4a05adadc29a        dolittle/mongo:4.2.2   "docker-entrypoint.s…"   2 minutes ago       Up 2 minutes        0.0.0.0:27017->27017/tcp   objective_mendeleev
```

### Interacting with the MongoDB server
To interact with the MongoDB server, you can use your usual tool like [MongoDB Compass Community](https://www.mongodb.com/products/compass) or [Robo 3R](https://robomongo.org), or use the MongoDB shell embedded in the container with the command:
```bash
$ docker exec -it objective_mendeleev mongo
MongoDB shell version v4.2.2
connecting to: mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb
Implicit session: session { "id" : UUID("89edffef-b042-4d93-87eb-eaa74bcb030c") }
MongoDB server version: 4.2.2
 ...
rs0:PRIMARY> 
```
The `rs0:PRIMARY` shown in the prompt indicates that the server is the master of the single server replica set called `rs0`.
