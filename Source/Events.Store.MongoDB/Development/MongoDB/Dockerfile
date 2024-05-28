FROM mongo:7.0.2-bionic

COPY replicaset-initiate.js /docker-entrypoint-initdb.d/

CMD ["mongod", "--replSet", "rs0"]
