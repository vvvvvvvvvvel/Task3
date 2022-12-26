#!/bin/bash

set -e
run_cmd="dotnet BillingGrpcServer.dll --no-build -v d"

exec $run_cmd