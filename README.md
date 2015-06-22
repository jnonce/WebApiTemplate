# WebApiTemplate

This is a template project for WebApi services.
It includes the basic structure for an Api service.

Current features:

* Http hosting via Microsoft Owin and HttpListener
* Routing, model serialization, and binding by WebApi
* Versioned Api routing
* Logging via [Serilog](https://github.com/serilog/serilog)
  * Each incoming request
  * Errors or warnings seen by WebApi or Owin
* Validation via [FluentValidation](https://github.com/JeremySkinner/FluentValidation)
* Dependency Injection via [Autofac](https://github.com/autofac/Autofac)
* API schema info via [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle) - (Swagger)[http://swagger.io/] style
* Tests via mstest
* Mocking via [Moq](https://github.com/Moq/moq4)
