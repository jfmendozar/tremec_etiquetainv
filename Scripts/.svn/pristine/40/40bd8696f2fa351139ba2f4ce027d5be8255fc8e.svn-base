IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spNumerosParteAgregar')
	DROP PROCEDURE [dbo].[spNumerosParteAgregar]
GO


-- =============================================
-- Author:		Francisco Mendoza	
-- Create date: 23/05/2018
-- Description:	Agrega información en la tabla NumerosParte
-- History:
-- <30-05-2018> <lk.centeno> <Cambiar INSERT porque se actualizo la estructura de la Tabla>
-- =============================================
CREATE PROCEDURE spNumerosParteAgregar
@pNumParte VARCHAR(30),
@pDescripcion VARCHAR(500),
@pIdUsuarioActualiza INT

AS
BEGIN
  BEGIN TRY
  BEGIN TRAN
	
	-- DECLARAR de Variables
	DECLARE @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	-- CREAR registro
	INSERT INTO NumerosParte(NumParte, Descripcion, IdUsuarioActualiza, FechaActualiza)
	VALUES (@pNumParte, @pDescripcion, @pIdUsuarioActualiza, GETDATE())
	-- COMPROMETER la transacción
	COMMIT TRAN

  END TRY
  BEGIN CATCH
	-- DESHACER trasacción si ocurre algun error.
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARAR variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT
	-- ERROR
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Agregar Numeros de Parte.'
	SET @vIncidente = 'Error al insertar datos en la tabla NumerosParte.'
	SET @vEsError = 1
	-- GUARDA el log en la tabla de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_024_Crea_spNumerosParteAgregar.sql', 'Script que inserta los registros de la tabla NumerosParte.', GETDATE())
GO
