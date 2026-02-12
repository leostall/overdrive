CREATE OR ALTER TRIGGER tr_update_address
ON address
AFTER UPDATE
AS
BEGIN
    UPDATE address
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE address.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_customer
ON customer
AFTER UPDATE
AS
BEGIN
    UPDATE customer
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE customer.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_vehicle
ON vehicle
AFTER UPDATE
AS
BEGIN
    UPDATE vehicle
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE vehicle.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_branch
ON branch
AFTER UPDATE
AS
BEGIN
    UPDATE branch
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE branch.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_stock
ON stock
AFTER UPDATE
AS
BEGIN
    UPDATE stock
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE stock.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_employee
ON employee
AFTER UPDATE
AS
BEGIN
    UPDATE employee
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE employee.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_part
ON part
AFTER UPDATE
AS
BEGIN
    UPDATE part
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE part.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_sale
ON sale
AFTER UPDATE
AS
BEGIN
    UPDATE sale
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE sale.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_sale_item
ON sale_item
AFTER UPDATE
AS
BEGIN
    UPDATE sale_item
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE sale_item.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_service_order
ON service_order
AFTER UPDATE
AS
BEGIN
    UPDATE service_order
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE service_order.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_service_order_item
ON service_order_item
AFTER UPDATE
AS
BEGIN
    UPDATE service_order_item
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE service_order_item.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_status
ON status
AFTER UPDATE
AS
BEGIN
    UPDATE status
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE status.id = i.id;
END;
GO

CREATE OR ALTER TRIGGER tr_update_payment
ON payment
AFTER UPDATE
AS
BEGIN
    UPDATE payment
    SET edited_at = CURRENT_TIMESTAMP
    FROM inserted i
    WHERE payment.id = i.id;
END;
GO