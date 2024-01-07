#!/usr/bin/bash

mkdir -p /tmp/talo
cd /tmp/talo
rm -r export
rm -r docs
rm .talo
clear

sleep 5
echo -e "$ # Welcome to a short demo of talo" | pv -qL 20
sleep 1

clear
echo -e "$ # Initialize ADR" | pv -qL 20
echo -e "$ talo init adr --location docs/adr" | pv -qL 20
talo init adr --location docs/adr

sleep 3

clear
echo -e "$ # List all initialized record types" | pv -qL 20
echo -e "$ talo list" | pv -qL 20
talo list

sleep 2

echo -e "$ # List all registered ADRs" | pv -qL 20
echo -e "$ # (adr command creates the first ADR automatically)" | pv -qL 20
echo -e "$ talo list adr" | pv -qL 10
talo list adr

sleep 4

clear
echo -e "$ # Let's add some ADRs" | pv -qL 20

echo -e "$ talo add adr --title \"Use event-based architecture\" --status Accepted" | pv -qL 30
talo add adr --title "Use event-based architecture" --status Accepted

echo -e "$ talo add adr --title \"Use MS SQL database\" --status Accepted" | pv -qL 30
talo add adr --title "Use MS SQL database" --status Accepted

sleep 1

clear
echo -e "$ # Let's create a new ADR that **supersedes** a previous ADR (number 3):" | pv -qL 20
echo -e "$ talo add adr --title \"Use PostgresSQL for all database needs\" --status Accepted --supersedes 3" | pv -qL 20
talo add adr --title "Use PostgresSQL for all database needs" --status Accepted --supersedes 3

sleep 2

echo -e "$ # List ADRs again to see how the statuses look:" | pv -qL 20
echo -e "$ # Note the status of ADR number 3:" | pv -qL 20
echo -e "$ talo list adr" | pv -qL 20
talo list adr

sleep 5

echo -e "$ # Observe the contents of the new ADR (number 4)" | pv -qL 20
echo -e "$ # (it has two rows in the status table)" | pv -qL 20
echo -e "$ head docs/adr/ADR0004-use-postgressql-for-all-database-needs.md" | pv -qL 30
head docs/adr/ADR0004-use-postgressql-for-all-database-needs.md

sleep 5

clear
echo -e "$ # Updating status" | pv -qL 20
sleep 1
echo -e "$ # You can use the \"revise\" command to update the status of a record." | pv -qL 20
echo -e "$ # You need to specify the number and the new status:" | pv -qL 20
echo -e "$ talo revise adr --number 2 --status Obsolete" | pv -qL 20
talo revise adr --number 2 --status Obsolete

sleep 1

echo -e "$ # Let's see how that looks:" | pv -qL 20
echo -e "$ talo list adr" | pv -qL 20
talo list adr

sleep 4

clear
echo -e "$ # Linking" | pv -qL 20
sleep 1
echo -e "$ # Some more ADRs first:" | pv -qL 20
echo -e "$ talo add adr --title \"Store configuration in files\" --status Accepted" | pv -qL 30
talo add adr --title "Store configuration in files" --status Accepted
echo -e "$ talo add adr --title \"Use dashes in file names\" --status Accepted" | pv -qL 30
talo add adr --title "Use dashes in file names" --status Accepted

sleep 1

echo -e "$ # talo can link two records to each other. For example:" | pv -qL 20
echo -e "$ talo link adr --source 6 --source-status \"Amends\" --destination 5 --destination-status \"Amended by\"" | pv -qL 10
talo link adr --source 6 --source-status "Amends" --destination 5 --destination-status "Amended by"
echo -e "$ # This updated the status tables of both records. Check it out:" | pv -qL 20
echo -e "$ talo list adr" | pv -qL 20
talo list adr

sleep 5

clear
echo -e "$ # Exporting" | pv -qL 20
sleep 1
echo -e "$ # You can use the \"export\" command to export your records as HTML." | pv -qL 20
echo -e "$ # talo will create hyperlinks between records whenever possible" | pv -qL 20
echo -e "$ talo export --types adr" | pv -qL 20
talo export --types adr
echo -e "$ # Let's list the files (note 'index.html')" | pv -qL 20
echo -e "$ ls -al ./export/adr" | pv -qL 20
ls -al ./export/adr
sleep 2
echo -e "$ # And browse the files:" | pv -qL 20
echo -e "$ firefox -new-window ./export/adr/index.html" | pv -qL 20
firefox -new-window ./export/adr/index.html
sleep 10

clear
echo -e "$ # This short demo showcases a subset of talo's functionality." | pv -qL 20
echo -e "$ # talo can do more:" | pv -qL 20
echo -e "$ # - you can define your own document types and manage them just like the built-in types" | pv -qL 20
echo -e "$ # - you can define your own templates for any document type" | pv -qL 20
echo -e "$ # Please refer to project README and \"talo --help\" for other functionality" | pv -qL 20

sleep 5

