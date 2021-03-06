IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spPerfilAgregar')
	DROP PROCEDURE [dbo].[spPerfilAgregar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  07/06/2017
-- Description:	 Agregar perfil nuevo
-- =============================================
CREATE PROCEDURE [dbo].[spPerfilAgregar]
@pNombre VARCHAR(100),
@pClave VARCHAR(30),
@pIdUsuarioActualiza INT,
@pIdPerfil INT OUTPUT
AS

BEGIN
  BEGIN TRY
	BEGIN TRAN

	-- VALIDA si el nombre del perfil existe
	IF EXISTS (SELECT * FROM Perfil WHERE Nombre = @pNombre)
	  BEGIN
		-- Resultado
		SELECT 'EXISTE'
	  END
	ELSE
	  BEGIN
		INSERT INTO Perfil (Nombre, Clave, IdUsuarioActualiza, FechaActualizado)
		VALUES (@pNombre, @pClave, @pIdUsuarioActualiza, GETDATE())
		-- RECUPERA el ID del perfil que se ha creado
		SET @pIdPerfil = (SELECT SCOPE_IDENTITY())
		-- Resultado
		SELECT 'OK'
	  END

	COMMIT TRAN
  END TRY
  BEGIN CATCH
	-- CANCELAR movimiento
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARAR variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Agregar perfil.'
	SET @vIncidente = 'Error al insertar datos en la tabla Perfil.'
	SET @vEsError = 1
	-- GUARDAR en el log de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_012_Crea_spPerfilAgregar.sql', 'Script que agrega un nuevo perfil a la tabla Perfiles.', GETDATE())
GO