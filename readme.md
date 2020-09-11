# Moonpig Engineering recruitment test

Welcome to the Moonpig engineering test. Here at Moonpig we really value
quality code, and this test has been designed to allow you to show us how 
you think quality code should be written. 

To allow you to focus on the design and implementation of the code we have 
added all the use cases we expect you to implement to the bottom of the 
instructions. In return we ask that you make sure your implementation 
follows all the best practices you are aware of, and that at the end of it, 
the code you submit, is code you are proud of. 

We have not set a timelimit, we prefer that you spend some extra time to get
it right and write the highest quality code you can. Please feel free to make
any changes you want to the solution, add classes, remove projects etc. If you
change the request or response please update the example in the section below.

For bonus points, commit regularly and include the .git folder in your
submission. This will allow us to follow the evolution of your solution.

When complete please upload your solution and answers in a .zip to the google
drive link provided to you by the recruiter.

## Programming Exercise - Moonpig Post Office

You have been tasked with creating a service that calculates the estimated 
despatch dates of customers' orders. 

An order consists of an order date and a collection of products that a 
customer has added to their shopping basket. 

Each of these products is supplied to Moonpig on demand through a number of 
3rd party suppliers.

As soon as an order is received by a supplier, the supplier will start 
processing the order. The supplier has an agreed lead time in which to 
process the order before delivering it to the Moonpig Post Office.

Once the Moonpig Post Office has received all products in an order it is 
despatched to the customer.

**Assumptions**:

1. Suppliers start processing an order on the same day that the order is 
	received. For example, a supplier with a lead time of one day, receiving
	an order today will send it to Moonpig tomorrow.


2. For the purposes of this exercise we are ignoring time i.e. if a 
	supplier has a lead time of 1 day then an order received any time on 
	Tuesday would arrive at Moonpig on the Wednesday.

3. Once all products for an order have arrived at Moonpig from the suppliers, 
	they will be despatched to the customer on the same day.

### Part 1 

When the /api/DespatchDate endpoint is hit return the despatch date of that 
order.

### Part 2

Moonpig Post Office staff are getting complaints from customers expecting 
packages to be delivered on the weekend. You find out that the Moonpig post
office is shut over the weekend. Packages received from a supplier on a weekend 
will be despatched the following Monday.

Modify the existing code to ensure that any orders received from a supplier
on the weekend are despatched on the following Monday.

### Part 3

The Moonpig post office is still getting complaints... It turns out suppliers 
don't work during the weekend as well, i.e. if an order is received on the 
Friday with a lead time of 2 days, Moonpig would receive and dispatch on the 
Tuesday.

Modify the existing code to ensure that any orders that would have been 
processed during the weekend resume processing on Monday.

---

Parts 1 & 2 have already been completed albeit lacking in quality. Please review
the code, document the problems you find (see question 1), and refactor into
what you would consider quality code. 

Once you have completed the refactoring, extend your solution to capture the 
requirements listed in part 3.

Please note, the provided DbContext is a stubbed class which provides test 
data. Please feel free to use this in your implementation and tests but do 
keep in mind that it would be switched for something like an EntityFramework 
DBContext backed by a real database in production.

While completing the exercise please answer the questions listed below. 
We are not looking for essay length answers. You can add the answers in this 
document.

## Questions

Q1. What 'code smells' / anti-patterns did you find in the existing 
	implemention of part 1 & 2?

Api:
* DbContext was instantiated inside the controller instead of being injected in
* Controller derived from Microsoft.AspNetCore.Mvc.Controller which added unnecessary View support
* Controller action had too much business logic, not very lightweight or Single Responsibility
* Variable names not descriptive
* ID variable should have been camel case, while _mlt was a (unnecessarily) public variable using a private variable naming convention

Data:
* Contained bad data - there was a product that linked to a non-existent supplier

Tests:
* Not testing individual units
* Controller instantiated in every test - repeated code
* Lots of individual tests that were very similar, again meaning repeated code
* No mocking
* Use of DateTime.Now meant that the tests would fail on certain days

Q2. What best practices have you used while implementing your solution?

* Lightweight controller only performing logic relating to the request and response. Has no knowledge of any business logic thanks to use of MediatR.
* Used dependency injection to ensure classes had no knowledge of concrete implementations where this was not necessary. This also allowed for the use of mocking in the unit tests.
* Used interfaces where appropriate
* Separated methods where appropriate for clarity and to ensure Single Responsibility principle adhered to.
* All database calls contained in repositories, so not tightly coupled to rest of the solution. This means swapping the current DbContext for an EntityFramework one should not have an effect on the rest of the code.
* Descriptive variable and method names, and stuck to same naming convention throughout
* Added validation to avoid a call getting half way through processing before discovering that the request data was not valid.
* Added problem details middleware to be more aligned to RFC 7807, that outlines the standard of error responses. Also ensures these are consistent for every response.
* Removed faulty data to avoid a 500 error being returned to the client after some processing may have already gone ahead. Tagged it with a todo comment, which should be picked up by code quality software so it can't be forgotten about. In a real scenario, resolving this properly, by finding the correct supplier or removing the product for good, would be top priority.
* Complete set of unit tests that ensure any future changes don't break the existing functionality. Tests are combined where appropriate and are completely independent.

Q3. What further steps would you take to improve the solution given more time?

* Add integration tests to ensure working end to end
* Add a Swagger front end to provide living documentation and an easily method to call the endpoint
* Add logging to collect data on how the API is used and to help with debugging any production errors
* Add a custom binder so the list query string can be sent via a comma separated string
* Find out from the client if any request or response headers would be useful to them, and add these
* Add some form of authentication to ensure only the right people have access

Q4. What's a technology that you're excited about and where do you see this 
    being applicable? (Your answer does not have to be related to this problem)
	
A new technology I am excited about is Postman's newly launched web client. It contains most of the features of the desktop version but is now accessible inside a browser. This will mean no need to install Postman onto any device I wish to use it on - instead I can simply navigate to the URL and get started quickly. One feature that sounds especially useful is that everything in the tool now has a URL. This will make collaboration between developers far easier as they can simply send a link to a resource for their co-worker to view. Using the desktop version in the past, if another developer wanted my thoughts on an API response, I would have to recreate their request on my own machine. This new browser interface would eliminate the need for that, allowing me to quickly respond to their query without wasting time on any setup. The new tool is applicable for any API work, including any future work being done in this solution.

## Request and Response Examples

Please see examples for how to make requests and the expected response below.

### Request

The service is setup as a Web API and takes a request in the following format

~~~~ 
GET /api/DespatchDate?ProductIds={product_id}&orderDate={order_date} 
~~~~

e.g.

~~~~ 
GET /api/DespatchDate?ProductIds=1&orderDate=2018-01-29T00:00:00
GET /api/DespatchDate?ProductIds=2&ProductIds=3&orderDate=2018-01-29T00:00:00 
~~~~

### Response

The response will be a JSON object with a date property set to the resulting 
Despatch Date

~~~~ 
{
    "date" : "2018-01-30T00:00:00"
}
~~~~ 

## Acceptance Criteria

### Lead time added to despatch date  

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 02/01/2018  

**Given** an order contains a product from a supplier with a lead time of 2 days  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Wednesday - 03/01/2018  

### Supplier with longest lead time is used for calculation

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order also contains a product from a different supplier with a lead time of 2 days  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Wednesday - 03/01/2018  

### Lead time is not counted over a weekend

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 08/01/2018  

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Saturday - 06/01/18  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 09/01/2018  

**Given** an order contains a product from a supplier with a lead time of 1 days  
**And** the order is place on a Sunday - 07/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 09/01/2018  

### Lead time over multiple weeks

**Given** an order contains a product from a supplier with a lead time of 6 days  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 15/01/2018  

**Given** an order contains a product from a supplier with a lead time of 11 days  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 22/01/2018