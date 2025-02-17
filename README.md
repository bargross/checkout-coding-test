# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

### Design Decisions

#### Models

##### PostPaymentRequest

###### CardNumberLastFour & Cvv
- In order to perform validation correctly, I decided to change the card number to a string type as is more flexible but it also adds multiple permutations to the possible param, so validation needs to be correct.
- valdating the values is easier as well as it adds more flexibity in tests after.

###### Required Attribute was added on the properties to avoid manual validation for simple things such as checking for nulls, empty, etc...

###### Status
 - I decided to change this property to a string in the response models to avoid overcomplicating dezerialization (AKA errors.)
 - String is also more flexible, mainly because as you expand an enum in one service, it can be come a problem in architectures such as Event Driven (Pubsub/Kafka, etc...), with consumer contracts, especially if you have contract tests. 

##### Persistence Models

- I decided to separate what we return/request from the actual resource to simplify the process (in a sense).
- Added two additional extensions to map to and between these models.
  - this also adds the capability to test these mappings in isolation (unit tests) if required.

##### PaymentProcessor

- In order to have a clean controller, who's responsability is just orchestrate the requests and handle the responses appropriately, I decided to split the responsability of 'who validates and stores these details' from the controller
- The level of abstraction used here I believe is necessary as it makes the code more readable and simplifies the implementation as well as it separates concerns from each class and assigns a single responsability to each of these.

##### PaymentGuard

This class was more my own flare to divide the validation responsability from the processer to ensure cleanliness of the implementation and add the capability to tests these validation functions in isolation if required.

##### Other

- The ProcessPayment endpoint returns status code 502 whenever the bank validator returns a 503 as it is an appropriate response to return when a downstream service is not available.
- The Cancellation token is a standard pattern in case the client cancels the request, there's no need to keep processing the request in either the API gateway or downstream services.

##### Integration Tests

- The integration tests is not currently using the Bank Provider, instead I decided to mock the bank validation client to save time.
- The client is not setup using the appsettings file, it should as is more scalable, we can add multiple clients in there and setup the clients during startup.
- Client calls timeout, On average I tend to add the appropriate timeout depending on what we're aiming for architecturatlly, as in, if we expect the calls should not take more than lets say 15s, as a precaution I tend to go with the highest possible to give the request time to complete. in this case I chose 10s but it can be lower since everything is local.

##### Test Naming strategy

[Action]_[State]_[Result]
