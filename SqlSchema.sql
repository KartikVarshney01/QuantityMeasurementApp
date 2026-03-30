CREATE DATABASE QuantityMeasurementAppDB;

USE QuantityMeasurementAppDB;


-- Step 1: Create database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'QuantityMeasurementAppDB')
    CREATE DATABASE QuantityMeasurementAppDB;
GO

USE QuantityMeasurementAppDB;
GO

-- Step 2: Create table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'quantity_measurements')
BEGIN
    CREATE TABLE quantity_measurements
    (
        id               INT           IDENTITY(1,1) PRIMARY KEY,
        operation        NVARCHAR(50)  NOT NULL,
        first_value      FLOAT         NOT NULL DEFAULT 0,
        first_unit       NVARCHAR(50)  NULL,
        second_value     FLOAT         NOT NULL DEFAULT 0,   -- 0 for Convert (single-operand)
        second_unit      NVARCHAR(50)  NULL,                 -- NULL for Convert
        result_value     FLOAT         NOT NULL DEFAULT 0,
        measurement_type NVARCHAR(50)  NULL,
        is_error         BIT           NOT NULL DEFAULT 0,
        error_message    NVARCHAR(500) NULL,
        created_at       DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Step 3: sp_SaveMeasurement
-- Parameters for second_value and result_value default to 0
-- so Convert (single-operand) never sends NULL into a NOT NULL column
IF OBJECT_ID('sp_SaveMeasurement', 'P') IS NOT NULL
    DROP PROCEDURE sp_SaveMeasurement;
GO

CREATE PROCEDURE sp_SaveMeasurement
    @operation        NVARCHAR(50),
    @first_value      FLOAT         = 0,
    @first_unit       NVARCHAR(50)  = NULL,
    @second_value     FLOAT         = 0,      -- 0 when no second operand (Convert)
    @second_unit      NVARCHAR(50)  = NULL,   -- NULL when no second operand (Convert)
    @result_value     FLOAT         = 0,
    @measurement_type NVARCHAR(50)  = NULL,
    @is_error         BIT           = 0,
    @error_message    NVARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO quantity_measurements
    (
        operation, first_value, first_unit,
        second_value, second_unit,
        result_value, measurement_type,
        is_error, error_message
    )
    VALUES
    (
        @operation, @first_value, @first_unit,
        ISNULL(@second_value, 0), @second_unit,
        ISNULL(@result_value, 0), @measurement_type,
        @is_error, @error_message
    );
END
GO

-- Step 4: sp_GetAllMeasurements
IF OBJECT_ID('sp_GetAllMeasurements', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllMeasurements;
GO

CREATE PROCEDURE sp_GetAllMeasurements
AS
BEGIN
    SELECT id, operation, first_value, first_unit,
           second_value, second_unit, result_value,
           measurement_type, is_error, error_message, created_at
    FROM quantity_measurements
    ORDER BY created_at DESC;
END
GO

-- Step 5: sp_GetMeasurementsByOperation
IF OBJECT_ID('sp_GetMeasurementsByOperation', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMeasurementsByOperation;
GO

CREATE PROCEDURE sp_GetMeasurementsByOperation
    @operation NVARCHAR(50)
AS
BEGIN
    SELECT id, operation, first_value, first_unit,
           second_value, second_unit, result_value,
           measurement_type, is_error, error_message, created_at
    FROM quantity_measurements
    WHERE operation = @operation
    ORDER BY created_at DESC;
END
GO

-- Step 6: sp_GetMeasurementsByType
IF OBJECT_ID('sp_GetMeasurementsByType', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMeasurementsByType;
GO

CREATE PROCEDURE sp_GetMeasurementsByType
    @measurement_type NVARCHAR(50)
AS
BEGIN
    SELECT id, operation, first_value, first_unit,
           second_value, second_unit, result_value,
           measurement_type, is_error, error_message, created_at
    FROM quantity_measurements
    WHERE measurement_type = @measurement_type
    ORDER BY created_at DESC;
END
GO

-- Step 7: sp_GetTotalCount
IF OBJECT_ID('sp_GetTotalCount', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTotalCount;
GO

CREATE PROCEDURE sp_GetTotalCount
AS
BEGIN
    SELECT COUNT(*) AS TotalCount FROM quantity_measurements;
END
GO

-- Step 8: sp_DeleteAllMeasurements
IF OBJECT_ID('sp_DeleteAllMeasurements', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteAllMeasurements;
GO

CREATE PROCEDURE sp_DeleteAllMeasurements
AS
BEGIN
    DELETE FROM quantity_measurements;
END
GO




