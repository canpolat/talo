## Reset

Reset demo directory
```sh
mkdir -p /tmp/talo # change this path for your system
cd /tmp/talo       # same here
rm -r docs
rm .talo
clear
```

## Basics

Initialize ADR with custom template path  
(template path is optional, you can omit it if you want to use the built-in template)
```sh
clear
talo init adr --location docs/adr --template-path templates/adr.md
```

Initialize RFC with built-in template
```sh
clear
talo init rfc --location docs/rfc
```

List all initialized record types
```sh
clear
talo list
```

List all registered ADRs  
(adr command creates the first ADR automatically)
```sh
clear
talo list adr
```

List all registered RFCs (empty list)
```sh
clear
talo list rfc
```

Let's add some ADRs:
```sh
clear
talo add adr --title "Use event-based architecture" --status Accepted
talo add adr --title "Use MS SQL database" --status Accepted
```

This new ADR **supersedes** a previous ADR (number 3):
```sh
clear
talo add adr --title "Use PostgresSQL for all database needs" --status Accepted --supersedes 3
```

Let's list ADRs again to see how the statuses look:
```sh
clear
talo list adr
```

Observe the contents of the new ADR  
(it has two rows in the status table)
```sh
clear
cat docs/adr/ADR0004-use-postgressql-for-all-database-needs.md
```

If you want to use a custom template for a specific record, you can do so:
```sh
clear
talo add adr --title "Use GPL-3.0-or-later as license" --status Accepted --from-template templates/differentadr.md
```

The list command also has a flag to display the file path:
```sh
clear
talo list adr --include-file-path
```

Let's check the contents of newly created ADR to confirm that talo used the given custom template:
```sh
clear
cat docs/adr/ADR0005-use-gpl-3.0-or-later-as-license.md
```

## Updating status

You can use the `revise` command to update the status of a record.
You need to specify the number and the new status:
```sh
clear
talo revise adr --number 2 --status Obsolete
```

Let's see how that looks:
```sh
clear
talo list adr
```

## Linking 

Some more ADRs first:
```sh
clear
talo add adr --title "Store configuration in files" --status Accepted
talo add adr --title "Use dashes in file names" --status Accepted
```

talo can link two records to each other. For example:
```sh
clear
talo link adr --source 7 --source-status "Amends" --destination 6 --destination-status "Amended by"
```

This will update the status tables of both records.
```sh
clear
talo list adr
```

## Exporting

You can use the `export` command to export your records as HTML.
This can be useful if you want to deploy them to a web site so that a wider audience can access them.
```sh
clear
talo export --help
```

It will by default export all records. But you can specify a type to limit it:
```sh
clear
talo export --types adr
```
Let's check the output (note the 'index.html' file):
```sh
clear
ls -al ./export/adr
```

And browse the files:
```sh
firefox -new-window ./export/adr/index.html
```

## Configuration

If you want to use a custom template for your records, you can configure that per record type:
```sh
clear
talo config adr --template-path templates/differentadr.md
```

### Create your own record type

You can use the `config add` command to create a custom record type:
```sh
clear
talo config add --name prd --location docs/prd --template-path templates/prd.md --description "Product Requirement Document"
```

```sh
clear
talo list
```

talo has now registered a new command: `prd`  
You can perform all the same operations with it.  
Let's add a new record using the custom type:

```sh
clear
talo add prd --title "Shopping cart experience" --status "Under review"
```

And list all records of this type:
```sh
clear
talo list prd
```

## Help

```sh
clear
talo --help
```

```sh
clear
talo add adr --help
```

```sh
clear
talo config --help
```

```sh
clear
talo link rfc --help
```

```sh
clear
talo --version
```

