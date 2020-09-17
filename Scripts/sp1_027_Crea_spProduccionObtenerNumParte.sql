IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionObtenerNumParte')
	DROP PROCEDURE [dbo].[spProduccionObtenerNumParte]
GO


-- =============================================
-- Author:		Francisco Mendoza
-- Create date: 29/05/2018
-- Description:	Script que valida si existe el número de parte
-- History:
-- <07-06-2018> <lk.centeno> <Se actualiza la consulta para que valide si existe el NumParte en Produccion.>
-- <07-06-2018> <jf.mendoza> <Se agregó el campo de IdUsuarioActualiza.>
-- <13-06-2018> <jf.mendoza> <Se agregó el parametro Resultado en el SELECT>
-- <22-06-2018> <jf.mendoza> <Se agregó una condición else para obtener el resultado cuando no exista el número de parte>
-- =============================================
CREATE PROCEDURE spProduccionObtenerNumParte 
@pNumParte VARCHAR(30),
@pIdUsuarioActualiza INT
AS
BEGIN
  BEGIN TRY

    -- OBTENER Numeros de Parte
	IF EXISTS (SELECT N.NumParte FROM Produccion P
			   INNER JOIN NumerosParte N ON N.IdNumParte = P.IdNumParte
			   WHERE N.NumParte = @pNumParte AND TipoEtiqueta = 'Proveedor')
	  BEGIN
		SELECT 'OK' Resultado
	  END
	  ELSE
		BEGIN
		 SELECT 'NOEXISTE' AS Resultado
		END

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionObtenerNumParte'
  	SET @vIncidente = 'Error al obtener los registros de Números de parte.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_027_Crea_spProduccionObtenerNumParte.sql', 'Script que valida si existe el número de parte.', GETDATE())
GO
