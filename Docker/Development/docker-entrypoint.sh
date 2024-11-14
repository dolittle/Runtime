#!/bin/bash

echo "Starting MongoDB"



# Start MongoDB as mongodb user in the background
su -s /bin/bash mongodb -c "mongod --config /etc/mongod.conf --replSet rs0" &
MONGO_PID=$!

# Function to handle cleanup on exit
cleanup() {
    echo "Starting graceful shutdown..."

    # Step 1: Stop the runtime gracefully
    if [ -n "$DOTNET_PID" ]; then
        echo "Stopping runtime (PID: $DOTNET_PID)"
        kill -SIGTERM "$DOTNET_PID"  # Send SIGTERM to the .NET process
        wait "$DOTNET_PID"  # Wait for .NET process to terminate
        echo "runtime stopped."
    fi

    # Step 2: Stop MongoDB gracefully
    if [ -n "$MONGO_PID" ]; then
        echo "Stopping MongoDB (PID: $MONGO_PID)"
        kill -SIGTERM "$MONGO_PID"  # Send SIGTERM to MongoDB process
        wait "$MONGO_PID"  # Wait for MongoDB to terminate
        echo "MongoDB stopped."
    fi

    echo "Graceful shutdown completed."
}

# Trap SIGTERM and call cleanup
trap cleanup SIGTERM SIGINT


# Wait for MongoDB to start
until mongosh --eval "print(\"waited for connection\")"
do
    sleep 2
done

# Initialize replica set
mongosh --eval '
rsconf = {
    _id: "rs0",
    members: [
        {
            _id: 0,
            host: "localhost:27017",
            priority: 1
        }
    ]
};
rs.initiate(rsconf);
'

# Wait for replica set to be fully initialized
until mongosh --eval "rs.isMaster().ismaster"
do
    sleep 2
done

echo "MongoDB replica set initialized and ready. Starting Runtime"


dotnet Dolittle.Runtime.Server.dll &
DOTNET_PID=$!

# Wait for runtime to complete (or until terminated)
wait "$DOTNET_PID"

# If the runtime terminates or crashes, initiate cleanup
cleanup
