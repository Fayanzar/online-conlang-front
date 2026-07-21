#!/bin/bash
if [ -f .env ]; then
    set -o allexport
    source .env
    set +o allexport
else
    echo "Error: .env file not found."
    exit 1
fi

rm -rf ./bin ./obj
dotnet clean
dotnet tool restore
dotnet fable clean --yes
dotnet fable

npm ci
npm run build
rsync -a --delete dist/ $FE_DIR
