IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spUsuarioEditar')
	DROP PROCEDURE [dbo].[spUsuarioEditar]
GO


-- =============================================
-- Author	  : Antonio Gonzalez
-- Create date:	06/06/2017
-- Description:	Editar usuario
-- History:		
-- <03/08/2017> <Antonio Gonzalez> <Contraseña no cammbia si no la ingresa.>
-- =============================================
CREATE PROCEDURE [dbo].[spUsuarioEditar]
@pIdUsuario INT
,@pUsuario VARCHAR(30)
,@pNombre VARCHAR(100)
,@pContrasena VARCHAR(50) = NULL
,@pIdPerfil SMALLINT
,@pIdTipoUsuario TINYINT
,@pIdUsuarioActualiza INT

AS
BEGIN
  BEGIN TRY

	BEGIN TRAN

	IF EXISTS (SELECT * FROM Usuario WHERE Usuario = @pUsuario AND IdUsuario <> @pIdUsuario)
	  BEGIN
		IF EXISTS (SELECT * FROM Usuario WHERE Usuario = @pUsuario AND IdUsuario <> @pIdUsuario AND Activo = 0)
		  BEGIN
			EXEC spUsuarioEliminar @pIdUsuario, @pIdUsuarioActualiza
			SET @pIdUsuario = (SELECT IdUsuario FROM Usuario WHERE Usuario = @pUsuario)
		  END
		ELSE
		  BEGIN
			SET @pIdUsuario = NULL
			SELECT 'EXISTE'
		  END
	  END

	IF	@pIdUsuario IS NOT NULL
	  BEGIN
		UPDATE Usuario SET
			Nombre = @pNombre
			,Usuario = @pUsuario
			,Contrasenia = (CASE WHEN @pContrasena IS NULL THEN Contrasenia ELSE LOWER(dbo.fnEncriptarContrasenia(@pContrasena)) END)
			,IdPerfil = @pIdPerfil
			,IdUsuarioActualiza = @pIdUsuarioActualiza
			,FechaActualizado = GETDATE()
			,IDTIPOUSUARIO = @pIdTipoUsuario
			,Activo = 1
		WHERE IdUsuario = @pIdUsuario
		SELECT 'OK'
	  END

	COMMIT TRAN

  END TRY
  BEGIN CATCH
	-- CANCELAR movimientos
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARACION de variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Editar usuario.'
	SET @vIncidente = 'Error al actualziar datos en la tabla Usuario.'
	SET @vEsError = 1
	-- GUARADAR error en el log de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_017_Crea_spUsuarioEditar.sql', 'Script que edita los valores de un usuario especifico.', GETDATE())
GO