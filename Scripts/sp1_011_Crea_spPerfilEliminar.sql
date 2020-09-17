IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spPerfilEliminar')
	DROP PROCEDURE [dbo].[spPerfilEliminar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  07/06/2017
-- Description:	 Eliminar perfil
-- =============================================
CREATE PROCEDURE [dbo].[spPerfilEliminar]
@pIdPerfil INT,
@pIdUsuarioActualiza INT
AS

BEGIN
	BEGIN TRY
		BEGIN TRAN
			DECLARE @vFuncionalidad VARCHAR(50)
				,@vIncidente VARCHAR(100)
				,@vEsError BIT

			IF EXISTS (SELECT * FROM Usuario WHERE IdPerfil = @pIdPerfil)
				SELECT 'EXISTE'
			ELSE
			BEGIN
				DELETE FROM PerfilModuloAccion WHERE IdPerfil = @pIdPerfil
				DELETE FROM Perfil WHERE IdPerfil = @pIdPerfil
				SELECT 'OK'
			END

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;
		DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT
		SET @ErrMsg = ERROR_MESSAGE()
		SET @ErrSev = ERROR_SEVERITY()
		SET @ErrSt = ERROR_STATE()		
		
		SET @vFuncionalidad = 'Elimiar perfil.'
		SET @vIncidente = 'Error al actualziar datos en la tabla Perfil.'
		SET @vEsError = 1
		
		EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
		RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
	END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_011_Crea_spPerfilEliminar.sql', 'Script que Elimina un perfil especifico de la tabla Perfiles.', GETDATE())
GO