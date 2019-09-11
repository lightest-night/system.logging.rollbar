# Lightest Night
## Logging > Rollbar

Rollbar Client and config files to support the logging of data to Rollbar

#### How To Use
##### Registration
* Asp.Net Standard/Core Dependency Injection
  * Use the provided `services.AddRollbar()` method
  
* Other Containers
  * Register an instance of `RollbarClient` as the `IRollbarClient` type with a Transient Lifecycle

##### Usage
Create a new instance of the `LogData` class and then call the `IRollbarClient.Log(LogData logdata)` method providing the `LogData` instance.