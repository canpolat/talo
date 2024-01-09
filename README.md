# talo

talo is a CLI tool to manage your ADRs, RFCs, software design documents and more.

With talo, you can create, update, list, and export your documents. It supports
ADRs (Architecture Decision Records) and RFCs (Request for Comments) out of the box.
But you can define your own document types.

Listing and export features set talo apart from similar tools.

If you are not familiar with ADRs or RFCs, this blog post offers a brief introduction:
[Documenting Design Decisions using RFCs and ADRs](https://brunoscheufler.com/blog/2020-07-04-documenting-design-decisions-using-rfcs-and-adrs)

<div align="center" style="text-align: center;">

![GitHub License](https://img.shields.io/github/license/canpolat/talo?logo=github)
![Nuget](https://img.shields.io/nuget/v/Canpolat.Talo?logo=nuget)
![Dotnet Tool](https://img.shields.io/badge/dotnet-tool-blue)
![.NET Version](https://img.shields.io/badge/Version-.NET%208-%23512BD4?logo=.net&logoColor=%23512BD4)
![Nuget download count](https://img.shields.io/nuget/dt/Canpolat.Talo)
<br/>

<img style="vertical-align:middle" alt="Runs on Linux" src="./docs/assets/tux.svg" height=32 />
<img style="vertical-align:middle" alt="Runs on Windows" src="./docs/assets/windows-logo.svg" height=32 />
<br/>
Runs on Linux and Windows

[Features](#features) •
[Demo](#demo) •
[Installation](#installation) •
[Usage](#usage) •
[Samples](#samples) •
[Custom templates](#custom-templates)

</div>

## Features

- **Supports ADRs and RFCs out of the box.** Comes with built-in templates so you can start
writing immediately (you can also use your own templates, if you so desire).
- **Template support:** You can use your own templates for all document types (including the 
built-in ones).
- **Supports custom document types:** You are not limited to ADR and RFC. You can create
and use your own document types and talo will manage them for you too.
- **Support for updating document status:** You can use the `revise` command to update the
status of a document. It will keep the version table tidy. Alternatively, you can use the
`link` command to link documents to each other. See examples below.
- **Listing:** You can list all your documents with their latest statuses.
- **Exporting:** You can export your documents to HTML. talo will create an `index.html`
document that can be used to browse the documents. It will also create links between
documents whenever possible. See Export section below.

## Demo

The screencast below (2m40s) demonstrates a subset of talo's functionality for ADR document type.
All document types have the same features. So, you can perform these operations on any document type.

If you want to try it yourself, you can use the annotated script at [./docs/demo.md](./docs/demo.md).

![Demo](./docs/assets/demo.gif)

## Installation

### As dotnet tool
talo is released as a dotnet tool. If you already have **.NET Runtime 8.0** or later, issue the following command
to install talo globally:

```sh
dotnet tool install --global Canpolat.Talo
```

### As single executable

Single executables for Linux and Windows are available under [releases](https://github.com/canpolat/talo/releases).
You can download the binary executable from under Assets and start using talo.

## Usage

### Initialization

You can initialize document types selectively. You don't need to initialize the
types you are not interested in.

#### Initialize ADR

You can initialize your ADR workflow, by issuing the following command:

```sh
talo init adr
```

If you want to specify a directory location for your ADRs, use the
`--location` option:

```sh
talo init adr --location docs/adr
```

There is also a `--template-path` option if you want to use your own template
instead of the built-in one:

```sh
talo init adr --location docs/adr --template-path templates/adr.md
```

Check help for details:

```sh
talo init adr --help
```

#### Initialize RFC

Initialization for RFC is the same as ADR. By replacing `adr` with `rfc`
in the above section, you can initialize your RFC workflow:

```sh
talo init rfc --location docs/rfc
```

#### Bare initialization

If you don't want talo to initialize any of the built-in document types,
you can use `init` without any sub-commands:

```sh
talo init
```

This will create a configuration file (`.talo`) at the current working
directory. You can then configure your own document types and initialize
them. See below for more information.

### Listing your documents

#### Listing ADRs

talo can list all your documents (including your custom document types). To
list all registered document types, issue the following command:

```sh
talo list
```

If you want to see all documents of a certain type, you can use the associated
sub-command for that. For example, to list all your ADRs, you can try:

```sh
talo list adr
```

### Creating new documents

To add new documents, you can use the `add` command. For example:

```sh
talo add adr --title "Use event-based architecture" --status "Accepted"
```

If you want to apply a specific custom template to a _single_ document during
creation, you can use the `--from-template` option.

```sh
talo add adr --title "Use GPL-3.0-or-later as license" --status "Accepted" --from-template "templates/differentadr.md"
```

### Updating status of a document

Document statuses are stored in a table in the document. If you use talo to update
the status, it will also add a timestamp to the table for reference.

The `revise` command can be used to update document status. You can pass any text
as the new status.

```sh
talo revise adr --number 2 --status "Obsolete"
```

This will add status "Obsolete" as the last status of ADR number 2.

### Linking documents to each other

There are two ways to link two documents to each other.

The first is when creating a new document. `talo add` has the `--supersedes` option
to indicate that the new document is superseding an older one:

```sh
talo add adr --title "Use PostgresSQL for all database needs" --status "Accepted" --supersedes 3
```

The second method is more flexible as it allows you to specify the status text
you want to use:

```sh
talo link adr --source 7 --source-status "Amends" --destination 6 --destination-status "Amended by"
```

After this command, document number `7` will get a new status `Amends ADR0006` and
document number `6` will get `Amended by ADR0007`.

### Export

You can use the `export` command to export your documents to HTML. This can be useful
if you want to deploy them to a web site so that a wider audience can access them.

```sh
talo export --help
```

By default, it will export all documents. But you can specify types to limit the output:

```sh
talo export --types adr
```

This will create HTMl files at `./export/adr` (relative to `.talo` file).

Note that talo will create an `index.html` file to make browsing easier. It will
also create links between documents whenever possible (for example, a document will
have a link to the document that supersedes it, and vice versa).

### Configuration

The `config` command provides a means to configure the existing document types as well
as create new ones.

If you want to change the directory or template location of ADRs, you can do so by:

```sh
talo config adr --location docs/newlocation ----template-path templates/differentadr.md
```

Note that this will only update the configuration and the configuration will impact only
new documents. It will not move any existing files from old location to new. Nor will it
update the old documents with the new template. 

#### Create a custom document type

To create a new document type you need to provide a name (all-lowercase), a location and a
template path. You can also specify a description. For example:

```sh
talo config add --name "prd" --location "docs/prd" --template-path "templates/prd.md" --description "Product Requirement Document"
```

After this, `talo list` command will list `prd` among the supported document types. You
can immediately start using this new sub-command:

```sh
talo add prd --title "Shopping cart experience" --status "Under review"
```

And list:

```sh
talo list prd
```

You now have a new document type that has the same capabilities as the built-in types
ADR and RFC.

## Samples

The `samples` directory contains some alternatives to built-in templates. Built-in
templates are also included for convenience (in case you want to tweak them to your
liking).

## Custom templates

Please pay attention the following points when creating a custom template. Once these are
satisfied, talo will be happy to help.

### Title line

talo uses the title line to populate lists and collect metadata.
For that reason, the template needs to comply with the expected format.
Make sure to use this as the first line in your template:

```md
# {{SEQUENCE-NUMBER}}. {{TITLE}}
```

### Status section

talo uses the status section in the document to read and write status information.
Make sure to include it towards the top of your document:

```md
## Status

| Status                   | Time               |
|--------------------------|--------------------|
| {{STATUS}}               | {{TIME}}           |
```

## Credits

- To convert markdown files to HTML, talo uses [markdig](https://github.com/xoofx/markdig) with all advanced extensions activated.
