# Azure Functions Service Template

In this solution are used nuget libriries from [Microservice.Core](https://github.com/victor-lubn/Microservice.Core).

This repository contains a Visual Studio template designed to create different types of Azure Function services, including API, Job, Durable, and Orchestrator services. This template serves as a starting point for new services, providing the appropriate structure, dependencies, and [Microservices.Core](https://github.com/victor-lubn/Microservice.Core) libriries.

## Features

- **Modular Structure**: Pre-defined structure for various Azure Function services such as API, Job, Durable Function, and Orchestrator.
- **Core Libraries**: Includes essential microservices libraries and dependencies to streamline development.
- **Customization**: Users can easily create an empty Azure Function service and customize it as needed.
- **Consistency**: Ensures consistency across different services within a microservices architecture.

# Clean Architecture approach 

During the implementation of the _Api Template_ were following the best practices provided by the _**Clean Architecture**_ approach. The git hub to the sample project that was taken as an example you can find [here](https://github.com/jasontaylordev/CleanArchitecture) 

According to this approach, we have some layers in our solution:

- _Domain_ - this layer contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer;

- _Application_ - this layer contains all application logic. It it dependent on the domain layer, but has no dependencies on any other layer. This layer defines interfaces that are implemented by outside layers;

- _Infrastructure_ - this layer contains classes that are based on interfaces defined within the application layer;

- API - this layer contains the number of Azure functions that have HTTP triggers. This layer depends on both the Application and Infrastructure layers.

# Folder structure

In our solution we follow the folder structure that consists of the following directories:
- _scr_ - the directory where all the source code is located;
- _tests_ - the directory where all the unit-test projects are placed;
- _docs_ - the directory where we store all the necessary documentation related to the project;
- _deployment_ - the directory with all the PowerShell, Terraform scripts, etc. that are needed to provision the infrastructure.

Inside src folder there are three folders for all the projects:
- **_Domain_** - there are some entities, enums and contracts specific to the domain layer defined on this layer. 
- **_Api_** - it is the place where all the main logic is defined. Inside Api folder there are three main projects:
    - _Api_ - The web project where all the Azure function are located (Api layer). 
All the Azure functions are self-documented in our solution. For this purpose we use Aliencube.AzureFunctions.Extensions.OpenApi library. The git hub to this library you can find [here](https://github.com/aliencube/AzureFunctions.Extensions/blob/dev/docs/openapi.md)
On this layer we use _Mediator_ pattern to reduce the number of dependencies between Api layer and Application layer.
    - _App_ - Is it our application layer that contains all the business logic. 
All the application logic in our solution are separated according to the _CQRS_ patter. It means that we separate the models that are used for reading data and models used for writing data.
All the business logic in our solution are grouped by the entity. Inside UseCases folder you can find all the commands and queries grouped by the entities. 
    - _Data_ - the project that has all the Entity Framework Core entities and Database Context defined.
- **_Common_** - it contains all the logic that can be shared between different layers.

# Core Nuget packages 

In this solution we use several _Nuget packages_. These packages can be easily shared between different projects. 

All the packages are stored in the _Azure DevOps Artifacts_. To get access to this feed we use _Nuget config_ file with Personal Access Token. To get more information on how to use Nuget packages in our solution you can follow the [link](https://github.com/victor-lubn/Microservice.Core)

# Microservice architecture example

![image](https://github.com/user-attachments/assets/37627911-4f03-4a19-9628-4a8a514f2ddb)

# Using "dotnet new" to create new projects from templates

Before reading further is is recommended to quickly review an article to become more familiar with "dotnet new" templates.
https://docs.microsoft.com/en-ca/dotnet/core/tools/custom-templates
Other useful links are gathered at the end of this article.

## Templating engine features used in templates

- Renames project folders
- Renames project file (.csproj)
- Renames namespaces 
- Renames file names
- Renames constants
- Replaces placeholder with generated values
- Adding projects depending on provided options
- Automatically adds project to a solution using post-actions
- Excludes specific files and projects depending on parameters values
- Conditionals in source code and in project files to exclude dependencies, extra code, extra usings.

## Benefits of using "dotnet" new templates
- All configuration is localized in one single "template.json" file.
- All templates can be packed into single nuget file.
- Intellisense support
- Includes autogenerated help

## Installing templates

It is possible to install templates from nuget package "Microservice.Templates"
Please check how to get templates sources and build templates nuget pack in the "updating-templates" section.

After nuget package is built install templates from it using following command

```dotnet new -i Microservice.Templates.1.0.0.nupkg```

Check installed templates using command

```dotnet new -l```

Project templates:

![image](https://github.com/user-attachments/assets/ab4c3df6-85fd-416e-ac4f-2775e21ec49f)

all templates have prefix "hwn".

### Available templates

#### Item Templates
- Nuget.config
- Code Analysis rules (stylecop)

#### Project templates
- API
- Orchestrator
- Job
- Durable microservice

## Using templates

Template pack contains projects templates and item templates.
Project templates:
- hwnapi (web api)
- hwnorch (eventhub orchestrator)
- hwnjob (job microservice trigged by timer and which can execute long-running tasks)
- hwnms (a microservice implemented using durable functions to execute a workflow consisting of several activities executed sequentially.)

Some of templates have parameters. 
Use "dotnet new <template> - h" to get more info about templates parameters.

### Using templates help

dotnet new hwnms -h

### Examples

Below is a list of commands to create a solution which contains microservices created from all templates.

Open preferred shell and execute following dotnet CLI commands

First create empty solution
```dotnet new sln -o Lubn.Depot ```

Open created folder
```cd Lubn.Depot```

Add orchestrator microservice
```dotnet new hwnorch -o Lubn.Depot.Orchestrator```

Add durable microservices

```dotnet new hwnms -t EmailAdapter -o Lubn.Depot.EmailAdapter```

Add job

```dotnet new hwnjob -t FileProcessing -o Lubn.Depot.FileProcessing```

Add api based on sql (default)

```dotnet new hwnapi -o Lubn.Depot```

Add Nuget.config

```dotnet new hwnmsnuget```

Add stylecop code analysis rules

```dotnet new hwnstylecop```

Add dll with stubs used by microservices templates

```
mkdir lib
cd lib
dotnet new hwnstubdll
```

### Example of solution creation script

```dotnet new sln -o Lubn.Depot

cd Lubn.Depot

dotnet new hwnorch -o Lubn.Depot.Orchestrator
dotnet new hwnms -t EmailAdapter -o Lubn.Depot.EmailAdapter
dotnet new hwnjob -t FileProcessing -o Lubn.Depot.FileProcessing
dotnet new hwnapi -o Lubn.Depot

dotnet new hwnmsnuget
dotnet new hwnstylecop

mkdir lib
cd lib
dotnet new hwnstubdll

cd ..

dotnet restore

start "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe" Lubn.Depot.sln

```

# Uninstall templates

Run the command ```dotnet new -u``` to see the list of installed templates

Following command will uninstall all the templates located in the package.
```dotnet new -u Lueben.Microservice.Templates``` 


## Updating Templates

### Build
After making changes to templates open folder containing Lueben.TemplatePack.csproj project and run following command:

```dotnet pack```

Generated package is located in "bin" folder.

## Creating projects from templates in visual studio

Starting in Visual Studio 16.8 Preview 2

https://stackoverflow.com/questions/55506405/how-to-use-dotnet-new-template-in-visual-studio-2019

![image](https://github.com/user-attachments/assets/5ac337e2-f1af-4e4d-b155-4909fa8a8227)

NB! It is not possible to specify template parameters in visual studio. As a result default values for parameters will be used.

## links

https://github.com/dotnet/templating/wiki
https://github.com/dotnet/templating/wiki/Runnable-Project-Templates
https://github.com/dotnet/templating/wiki/Reference-for-template.json
https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates
https://github.com/dotnet/dotnet-template-samples


## how to rename filenames

https://github.com/dotnet/templating/issues/1238

