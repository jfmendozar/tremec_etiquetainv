IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spNumerosParteEditar')
	DROP PROCEDURE [dbo].[spNumerosParteEditar]
GO


-- =============================================
-- Author:		Francisco Mendoza
-- Create date: 23/05/2018
-- Description:	Edita Los campos de la tabla NumerosParte
-- History:
-- <30-05-2018> <lk.centeno> <Cambiar INSERT porque se actualizo la estructura de la Tabla>
-- =============================================
CREATE PROCEDURE spNumerosParteEditar
@pNumParte VARCHAR(30),
@pDescripcion VARCHAR(500),
@pIdUsuarioActualiza INT

AS
BEGIN
  BEGIN TRY

	BEGIN TRAN

	IF NOT EXISTS (SELECT * FROM NumerosParte WHERE NumParte = @pNumParte)
	  BEGIN
		-- NO EXISTE enviar mensaje
		SELECT 'NO EXISTE'
	  END
	ELSE
	  BEGIN
		-- ACTUALIZA registro
		UPDATE NumerosParte SET
		  NumParte = @pNumParte,
		  Descripcion = @pDescripcion,
		  IdUsuarioActualiza = @pIdUsuarioActualiza,
		  FechaActualiza = GETDATE()
		WHERE NumParte = @pNumParte
		-- PROCESO concluido
		SELECT 'OK'
	  END

	-- COMPROMETER la transacción
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
	SET @vFuncionalidad = 'Editar Numeros de Parte.'
	SET @vIncidente = 'Error al actualziar datos en la tabla NumerosParte.'
	SET @vEsError = 1
	-- GUARDAR el log en la tabla de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_025_Crea_spNumerosParteEditar.sql', 'Script que editar los registros de la tabla NumerosParte.', GETDATE())
GO