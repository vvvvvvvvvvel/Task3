#!/bin/bash

set -e
run_cmd="dotnet BillingAPI.dll --no-build -v d"

exec $run_cmd