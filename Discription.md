This is the Windows service “Task_Queue”, which accepts requests from the user to perform complex long-running tasks and sequentially executes them in a background process, taking into account the priority of the tasks.

The execution time of one task is X seconds. X is set in the registry parameter.
HKLM/Software/Task_Queue/Parameters/Task_Execution_Duration

The duration of one cycle of the first process is Y seconds. Y is set in the registry parameter.
HKLM/Software/Task_Queue/Parameters/Task_Claim_Check_Period

A process can execute a maximum of Z tasks at the same time. Z has a value from 1 to 3 (default value 1) and is set in the registry parameter
HKLM/Software/Task_Queue/Parameters/Task_Execution_Quantity

The service writes all its messages to the log
C:\Windows\Logs\TaskQueue_14-10-2024.log

In order to place a request - you need to create a new empty SubKey with the name Task_XXXX in the registry branch
HKLM/Software/Task_Queue/Claims
, where XXXXX is the new unique task number.

If the request is successfully accepted for processing - this means that the user-created registry subkey in the HKLM/Software/Task_Queue/Claims branch is deleted 
and an empty subkey with the name Task_0832-[....................]-Queued in the HKLM/Software/Task_Queue/ branch is created instead.
Square brackets mean progress bar, between them - 20 dots, each of which means 5 percent of the completed task.

The service also has error handling, errors and execution progress are displayed in the log file.

The service also has an installer program Setup1.exe

Linux-Service
When starting, the service should check for the presence of a text log KI-Kryvorot.log, if there is no log - create this file and write a message to it that the service has started successfully. 
Then, every 6 seconds, the service should write to the same file the result of executing the command "top -b -n 1 | grep \"MiB Mem\" | head -n 1", which outputs one line of text.
