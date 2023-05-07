# RPA Robot Project

## Overview
This RPA (Robotic Process Automation) project aims to automate workflows using a combination of activities, logging, and orchestrator communication. The project includes a set of functions implemented in C# to handle various aspects of workflow execution, logging, and file management.

## Features
The RPA robot offers the following key features:

1. **RunWorkFlow**: Loads and executes a workflow defined in XAML format. It utilizes the `ActivityXamlServices` and `WorkflowApplication` classes to load and run the workflow. Additionally, it allows for subscribing to events such as `Completed` and `OnUnhandledException` to handle workflow completion and unhandled exceptions, respectively.

2. **LoggingProcess**: Manages the logging process by sending log data to an orchestrator and processing the orchestrator process queue. It continuously checks for conditions to send log data and process the queue in an infinite loop. This function leverages the `LogQueue` and communicates with an orchestrator using asynchronous methods.

3. **OnFileCreated**: Acts as an event handler for the file creation event. It is triggered when a new file is created in a specified directory. This function logs the full path of the created file and optionally performs additional actions related to the workflow, such as copying the workflow, deleting the original file, or triggering workflow execution.

4. **CpyWorkFlow**: Copies the workflow file from a source directory to a destination directory. It utilizes the `File.Copy` method to perform the file copy operation. The function provides logging capabilities to track the outcome of the file copying process, including any encountered errors.

5. **DeleteeWorkFlow**: Deletes the workflow file at the specified path. It uses the `File.Delete` method to remove the file from the system. The function logs the outcome of the file deletion process, including any errors encountered during the deletion operation.

6. **StartWorkFlowThread**: Initiates a new thread to execute the workflow asynchronously. This function creates a new thread using the `Thread` class and starts its execution. It is typically used when executing the workflow in a separate thread is desired.

7. **ThreadMethod**: Represents the method executed by a separate thread to run the workflow. It sets the flag indicating that the last workflow execution is not yet done and invokes the `RunWorkFlow` function to start the workflow execution. Additionally, it provides the option to set the flag indicating that the last workflow execution is complete.

## Usage
To use the RPA robot functions, follow these steps:

1. Incorporate the provided functions into your existing C# project or create a new project.

2. Ensure that the required dependencies and libraries are properly referenced in your project.

3. Customize the functions according to your specific workflow automation needs.

4. Run the project and observe the workflow execution, logging process, and other relevant actions.

## Contributions
Contributions to this RPA robot project are welcome! If you have any suggestions, bug fixes, or additional features to propose, please feel free to submit a pull request or open an issue.

## Acknowledgements
We would like to express our gratitude to the developers and contributors of the libraries and frameworks used in this project. Their hard work and dedication have been instrumental in making this RPA robot possible.

