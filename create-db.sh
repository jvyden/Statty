#!/bin/bash
echo "Creating database...";
touch statty.db
sqlite3 statty.db < create.sql
echo "Created.";