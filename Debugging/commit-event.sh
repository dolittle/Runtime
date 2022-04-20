#!/bin/bash

grpcurl -plaintext -d @ localhost:50053 dolittle.runtime.events.EventStore/Commit <<EOM
{
  "callContext": {
    "executionContext": {
      "microserviceId": { "value": "wytdSj9UmkWrC+jpJAkyYA==" },
      "tenantId": { "value": "qI5fRG8a10Cy/HltupLcRA==" },
      "version": { "major": 0, "minor": 0, "patch": 0, "build": 0, "preReleaseString": "" },
      "correlationId": { "value": "qI5fRG8a10Cy/HltupLcRA==" },
      "claims": [],
      "environment": "Development"
    },
    "headId": { "value": "V6jYF/gFV0OLSiZ76NlDog==" }
  },
  "events": [
    {
      "eventType": {
        "id": { "value": "iGI0EEUwfEirbsiGUfwBNQ==" },
        "generation": 0
      },
      "eventSourceId": "event source",
      "public": false,
      "content": "{ \"hello\": \"world\" }"
    }
  ]
}
EOM
