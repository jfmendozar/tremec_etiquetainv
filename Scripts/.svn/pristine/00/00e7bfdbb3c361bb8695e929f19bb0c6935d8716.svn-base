IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'spValidarLenguaje')
  DROP PROCEDURE spValidarLenguaje 
GO


-- =============================================
-- Author:		Francisco Mendoza
-- Create date: 25/05/2018
-- Description:	Script que se encarga de crear una tabla temporal donde se puede consultar 
--				el lenguaje de la base de datos a la cual se le vera afectada con la carga masiva
-- =============================================
CREATE PROCEDURE spValidarLenguaje

AS
BEGIN

  BEGIN TRY
	
	BEGIN TRAN

	-- CREAR tabla temporal
	CREATE TABLE #TEMPLANGUAGE(lang_id INT, date_format VARCHAR(50), date_first INT, upgrade INT, name VARCHAR(50), alias VARCHAR(50), 
		months VARCHAR(200), shortmonths VARCHAR(200), [days] VARCHAR(200), Icid INT, msglangid INT)

	-- AGREGAR registros a la base de datos
	INSERT INTO #TEMPLANGUAGE EXEC sp_helplanguage 
	SELECT name, date_format 
	FROM #TEMPLANGUAGE 
	WHERE name = @@LANGUAGE 
	
	-- ELIMINAR tabla temporal
	DROP TABLE #TEMPLANGUAGE

	COMMIT TRAN

  END TRY
  BEGIN CATCH
	-- DESHACER transacción por algun error
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARARA variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	-- DEFINIR valores para las variables
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Agregar Numeros de Parte.'
	SET @vIncidente = 'Error al insertar datos en la tabla NumerosParte.'
	SET @vEsError = 1
	-- GUARDAR el log en la tabla de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_033_Crea_spValidarLenguaje.sql', 'Script que se encarga de crear una tabla temporal para la carga masiva.', GETDATE())
GO