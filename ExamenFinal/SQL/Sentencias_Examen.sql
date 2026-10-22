
-- =========================================================
-- EXAMEN FINAL
-- PROGRAMACION ORIENTADA A OBJETOS I
-- PROYECTO: POOI_EF_Vasquez
-- =========================================================


-- =========================================================
-- PREGUNTA 1: PROCEDIMIENTOS DE CONSUMIDORES
-- BASE DE DATOS: ExamenFinal
-- =========================================================

USE ExamenFinal;
GO


-- ---------------------------------------------------------
-- PROCEDIMIENTO: AGREGAR CONSUMIDOR
-- ---------------------------------------------------------

CREATE OR ALTER PROCEDURE dbo.usp_agrega_consumidor
    @dnicont CHAR(8),
    @nomcont VARCHAR(40),
    @apecont VARCHAR(40),
    @emailcont VARCHAR(250),
    @idpais CHAR(3),
    @mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF LEN(LTRIM(RTRIM(@dnicont))) <> 8
       OR @dnicont LIKE '%[^0-9]%'
    BEGIN
        SET @mensaje = 'ERROR: EL DNI DEBE TENER 8 DIGITOS NUMERICOS';
        RETURN;
    END;

    IF NULLIF(LTRIM(RTRIM(@nomcont)), '') IS NULL
       OR NULLIF(LTRIM(RTRIM(@apecont)), '') IS NULL
    BEGIN
        SET @mensaje = 'ERROR: NOMBRE Y APELLIDO SON OBLIGATORIOS';
        RETURN;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.tb_consumidor
        WHERE dnicont = @dnicont
    )
    BEGIN
        SET @mensaje = 'ERROR: EL DNI YA EXISTE';
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.tb_consumidor
            (dnicont, nomcont, apecont, emailcont, idpais)
        VALUES
            (@dnicont, @nomcont, @apecont, @emailcont, @idpais);

        SET @mensaje = 'CONSUMIDOR AGREGADO CORRECTAMENTE';
    END TRY
    BEGIN CATCH
        SET @mensaje = ERROR_MESSAGE();
    END CATCH;
END;
GO


-- ---------------------------------------------------------
-- PROCEDIMIENTO: ELIMINAR CONSUMIDOR
-- ---------------------------------------------------------

CREATE OR ALTER PROCEDURE dbo.usp_eliminar_consumidor
    @dnicont CHAR(8),
    @mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.tb_consumidor
        WHERE dnicont = @dnicont
    )
    BEGIN
        SET @mensaje = 'ERROR: EL CONSUMIDOR NO EXISTE';
        RETURN;
    END;

    BEGIN TRY
        DELETE FROM dbo.tb_consumidor
        WHERE dnicont = @dnicont;

        SET @mensaje = 'CONSUMIDOR ELIMINADO CORRECTAMENTE';
    END TRY
    BEGIN CATCH
        SET @mensaje = ERROR_MESSAGE();
    END CATCH;
END;
GO


-- ---------------------------------------------------------
-- PROCEDIMIENTO: LISTAR CONSUMIDORES
-- ---------------------------------------------------------

CREATE OR ALTER PROCEDURE dbo.usp_listar_consumidor
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        dnicont,
        nomcont,
        apecont,
        emailcont,
        idpais
    FROM dbo.tb_consumidor
    ORDER BY apecont, nomcont;
END;
GO


-- PRUEBA DE LISTADO DE CONSUMIDORES
EXEC dbo.usp_listar_consumidor;
GO



-- =========================================================
-- PREGUNTA 2: EMPLEADOS Y CANTIDAD DE PEDIDOS POR ANIO
-- BASE DE DATOS: BDNorthwind
-- =========================================================

USE BDNorthwind;
GO


-- ---------------------------------------------------------
-- PROCEDIMIENTO: PEDIDOS POR EMPLEADO Y ANIO
-- ---------------------------------------------------------

CREATE OR ALTER PROCEDURE dbo.usp_pedidos_por_empleado
    @ANIO INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.EmployeeID,
        E.FirstName,
        E.LastName,
        YEAR(O.OrderDate) AS Anio,
        COUNT(O.OrderID) AS CantidadPedidos
    FROM dbo.Employees AS E
    INNER JOIN dbo.Orders AS O
        ON E.EmployeeID = O.EmployeeID
    WHERE O.OrderDate >= DATEFROMPARTS(@ANIO, 1, 1)
      AND O.OrderDate < DATEFROMPARTS(@ANIO + 1, 1, 1)
    GROUP BY
        E.EmployeeID,
        E.FirstName,
        E.LastName,
        YEAR(O.OrderDate)
    ORDER BY
        E.LastName,
        E.FirstName;
END;
GO


-- PRUEBA PARA EL ANIO 1996
EXEC dbo.usp_pedidos_por_empleado @ANIO = 1996;
GO


-- PRUEBA PARA EL ANIO 1997
EXEC dbo.usp_pedidos_por_empleado @ANIO = 1997;
GO


-- =========================================================
-- FIN DEL SCRIPT DEL EXAMEN
-- =========================================================
