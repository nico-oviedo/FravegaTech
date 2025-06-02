# FravegaTech

*** INSTALLATION ***

1. Download source code from this repository and open FravegaTech.sln with Visual Studio 2022.

2. Configure the following as startup projects:
	a. BuyerService.API
	b. OrderServiceService.API
	c. ProductService.API
	
3. Build and run.

P.D: Also you can find postman collection to test all the solution endpoints.

** Dependencies:** MongoDB local database running at "mongodb://localhost:27017".


*** TECHNICAL DECISIONS ***

1. Each microservice has its own database.

2. When a new order is created, if any of its products or the buyer doesn't exist in the system (database), it will be created automatically.

3.1 Validation of the existence of a buyer by document number.
3.2 Validation of the existence of a product by SKU.