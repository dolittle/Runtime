#!/bin/bash

TEMPLATE="Documentation/References/compatibility.md"
GENERATED="$1"

if [ ! -f "$GENERATED" ]; then
    echo "The provided generated file '$GENERATED', is not valid!"
    exit 1
fi

if [ ! -f "$TEMPLATE" ]; then
    echo "The template file '$TEMPLATE', is not valid!"
    exit 1
fi

echo "Merging '$GENERATED' into '$TEMPLATE' ..."

HEAD=$(sed -ne '1,/<!-- BEGIN TABLE -->/p' <"$TEMPLATE")
TAIL=$(sed -ne '/<!-- END TABLE -->/,$p' <"$TEMPLATE")

echo "$HEAD" > "$TEMPLATE"
cat "$GENERATED" >> "$TEMPLATE"
echo "$TAIL" >> "$TEMPLATE"

git diff --quiet "$TEMPLATE"
CHANGED="$?"

function set_github_output {
    if [ "$CI" == "true" ]; then
      echo "::set-output name=$1::$2"
    fi
}

if [ "$CHANGED" -ne 0 ]; then 
    echo "The update has changed the compatibility table"
    set_github_output "changed" "true"
else
    echo "The update has not changed the compatibility table"
    set_github_output "changed" "false"
fi
