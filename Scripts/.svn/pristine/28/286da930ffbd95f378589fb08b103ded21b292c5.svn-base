IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'spConfiguracionConsultar')
	DROP PROCEDURE spConfiguracionConsultar
GO

-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 14/06/2018
-- Description:	Stored procedure que obtiene la configuración general de algunos campos en particular o bien de todos los valores.
--				<@pDatosConfigurados> <NULL | NombreDatoConfigurable1,NombreDatoCofigurable2,...,NombreDatoCofigurableN>
-- =============================================
CREATE PROCEDURE [dbo].[spConfiguracionConsultar]
@pDatosConfigurados VARCHAR(MAX) = NULL

AS
BEGIN

	IF(@pDatosConfigurados IS NULL)
	  BEGIN
		SELECT IdConfiguracion, Nombre, Valor, Visible, ExpresionRegular
		FROM Configuracion
	  END
	ELSE
	  BEGIN
		SELECT IdConfiguracion, Nombre, Valor, Visible, ExpresionRegular
		FROM Configuracion
		WHERE Nombre IN (SELECT Dato FROM [dbo].[fnCadenaATabla] (@pDatosConfigurados,','))
	  END

END
GO

INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_037_Crear_spConfiguracionConsultar.sql', 'Script que crea el SP que obtiene la configuración general de algun campo en particular o todos los valores.', GETDATE())
GO