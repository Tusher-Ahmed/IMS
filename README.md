### Project: Inventory Management System (IMS) with ASP.NET MVC 5, NHibernate, Entity Framework, and MSSQL 2019
An inventory management system (IMS) is a software or hardware-based solution used by businesses to efficiently track, control, and manage their inventory or stock of goods. It plays a crucial role in ensuring that a company has the right amount of products on hand to meet customer demand while minimising carrying costs and preventing stock outs or overstock situations.In this inventory management system where Garments companies act as suppliers, and inventory  purchasing clothes from the supplier and then the customers are purchasing the products from the inventory.

### Roles:

Admin,
Manager,
Staff,
Garments,
Shop 

### Inventory Operations:

Admin, Manager, and Staff roles manage inventory.
Manager can order products from Garments, generating an invoice.
Staff performs quality checks on received products.
Manager can modify product details and set prices.

### Garments Module:

Garments manufacture and add products.
View order history from the inventory.

### Product Ordering:

Products with set prices are added to the product page.
Registered shops can place orders with instant payment via Stripe.
Buying invoices are generated for shops.

### Order Status:

Order progresses through stages:
Order Accepted (Can be canceled by the shop with a valid reason),
Processing,
Shipped,
Delivered (Shop can return within seven days for valid reasons).

### Reports and Administration:

Admin/Manager can view buying and selling reports with specified dates or credentials.
User deactivation feature for Admin.

### Key Features:

Seamless ordering process from Garments to Shop with real-time payment.
Comprehensive order status tracking.
Flexibility for inventory management and product modifications.
Detailed reporting for business analysis.
User-friendly interface for efficient operations.

### Technologies Used:

ASP.NET MVC 5,
NHibernate,
Entity Framework,
MSSQL 2019,
Stripe for Payment Integration

This Inventory Management System streamlines the product lifecycle, ensuring a smooth journey from manufacturing to delivery while providing robust reporting and administrative controls.
