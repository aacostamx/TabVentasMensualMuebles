-- =============================================
-- Procedimiento AF0097_TabVentasMensualMuebles
-- =============================================
use Carteras
go

--****************************************************************************************
--****************************************************************************************						
--Se declara la variable fecha donde se almacenara la fecha del catFechas
declare @Fecha as smalldatetime
select @Fecha=Fecha from ODS.Dic2013.dbo.CatFechas

--Se calcula el el saldo, vencido y numero de operaciones de la Cartera de Muebles
if exists(select * from sysobjects where name ='tmp_VtasMensualMuebles00')drop table tmp_VtasMensualMuebles00
select month(FechaVenta)Mes,year(FechaVenta)Anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,
sum(cast(ltrim(rtrim(substring(dbo.fngeVencidoMuebles(@fecha,FechaVenta,ImporteVenta,InteresSobreCompra,PlazoVenta,AbonoMensual,SaldoALAFecha,Enganche),34,11))) as float))Vencido,
count(*)NumCuentas
into tmp_VtasMensualMuebles00
from ODS.Dic2013.dbo.MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc

--Se calcula el saldo en miles, y el vencido en miles
if exists(select * from sysobjects where name = 'tmp_VtasMensualMuebles') drop table tmp_VtasMensualMuebles
select Mes,Anio,Saldo,round(cast(Saldo as float)/1000,0)SaldoenMiles,Vencido,round(cast(Vencido as float)/1000,0)VencidoenMiles,NumCuentas
into tmp_VtasMensualMuebles
from tmp_VtasMensualMuebles00

--Se crea el campo Fecha con el nombre del mes y se le pega el año ejemplo: Mes:12, anio2013
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal') drop table tmp_VtasMensualMueblesFinal
select case
    when Mes=1 then 'Ene'+ cast(anio as char(4))
	when Mes=2 then 'Feb'+ cast(anio as char(4)) 
	when Mes=3 then 'Mar'+ cast(anio as char(4)) 
	when Mes=4 then 'Abr'+ cast(anio as char(4)) 
	when Mes=5 then 'May'+ cast(anio as char(4)) 
	when Mes=6 then 'Jun'+ cast(anio as char(4)) 
	when Mes=7 then 'Jul'+ cast(anio as char(4)) 
	when Mes=8 then 'Ago'+ cast(anio as char(4)) 
	when Mes=9 then 'Sep'+ cast(anio as char(4)) 
	when Mes=10 then 'Oct'+ cast(anio as char(4)) 
	when Mes=11 then 'Nov'+ cast(anio as char(4)) 
	when Mes=12 then 'Dic'+ cast(anio as char(4))
end Fecha,*
	into tmp_VtasMensualMueblesFinal
	from tmp_VtasMensualMuebles
	
--****************************************************************************************
--****************************************************************************************						
--Selecciona las ventas de la Cartera de Muebles donde el tipo de producto exista en el cátalogo de producto que no sea Negocio Afiliado COPPEL
if exists(select * from sysobjects where name ='tmp_VtasMensualMuebles_Coppel00')drop table tmp_VtasMensualMuebles_Coppel00
select month(FechaVenta)Mes,year(FechaVenta)Anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,
sum(cast(ltrim(rtrim(substring(dbo.fngeVencidoMuebles(@fecha,FechaVenta,ImporteVenta,InteresSobreCompra,PlazoVenta,AbonoMensual,SaldoALAFecha,Enganche),34,11))) as float))Vencido,
count(*)NumCuentas
into tmp_VtasMensualMuebles_Coppel00
from ODS.Dic2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=0)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc

--Se calcula el saldo en miles, y el vencido en miles
if exists(select * from sysobjects where name = 'tmp_VtasMensualMuebles_Coppel') drop table tmp_VtasMensualMuebles_Coppel
select Mes,Anio,Saldo,round(cast(Saldo as float)/1000,0)SaldoenMiles,Vencido,round(cast(Vencido as float)/1000,0)VencidoenMiles,NumCuentas
into tmp_VtasMensualMuebles_Coppel
from tmp_VtasMensualMuebles_Coppel00

--Se crea el campo mes con el formato MmmAAAA ejemplo: Ene2013 en la tabla de las ventas Coppel (no incluye Negocios Afiliados)
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_Coppel') drop table tmp_VtasMensualMueblesFinal_Coppel
select case
    when Mes=1 then 'Ene'+ cast(anio as char(4))
	when Mes=2 then 'Feb'+ cast(anio as char(4)) 
	when Mes=3 then 'Mar'+ cast(anio as char(4)) 
	when Mes=4 then 'Abr'+ cast(anio as char(4)) 
	when Mes=5 then 'May'+ cast(anio as char(4)) 
	when Mes=6 then 'Jun'+ cast(anio as char(4)) 
	when Mes=7 then 'Jul'+ cast(anio as char(4)) 
	when Mes=8 then 'Ago'+ cast(anio as char(4)) 
	when Mes=9 then 'Sep'+ cast(anio as char(4)) 
	when Mes=10 then 'Oct'+ cast(anio as char(4)) 
	when Mes=11 then 'Nov'+ cast(anio as char(4)) 
	when Mes=12 then 'Dic'+ cast(anio as char(4))
end Fecha,*
	into tmp_VtasMensualMueblesFinal_Coppel
	from tmp_VtasMensualMuebles_Coppel

	
--****************************************************************************************
--****************************************************************************************		
--Selecciona las ventas de la Cartera de Muebles donde el tipo de producto exista en el cátalogo de producto sea de Negocio Afiliado Negocios Afiliados
if exists(select * from sysobjects where name = 'tmp_VtasMensualMuebles_NegAfiliado00') drop table tmp_VtasMensualMuebles_NegAfiliado00
select month(FechaVenta)Mes,year(FechaVenta)Anio,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,
sum(cast(ltrim(rtrim(substring(dbo.fngeVencidoMuebles(@fecha,FechaVenta,ImporteVenta,InteresSobreCompra,PlazoVenta,AbonoMensual,SaldoALAFecha,Enganche),34,11))) as float))Vencido,
count(*)NumCuentas
into tmp_VtasMensualMuebles_NegAfiliado00
from ODS.Dic2013.dbo.MaeCarteraMuebles
where TipoProducto in (select ClaveTipoProducto from CatTipoProducto 
where Cartera='M' and NegAfiliado=1)
group by year(FechaVenta),month(FechaVenta)
order by anio desc, mes desc

--Se calcula el saldo en miles, y el vencido en miles
if exists(select * from sysobjects where name = 'tmp_VtasMensualMuebles_NegAfiliado') drop table tmp_VtasMensualMuebles_NegAfiliado
select Mes,Anio,Saldo,round(cast(Saldo as float)/1000,0)SaldoenMiles,Vencido,round(cast(Vencido as float)/1000,0)VencidoenMiles,NumCuentas
into tmp_VtasMensualMuebles_NegAfiliado
from tmp_VtasMensualMuebles_NegAfiliado00

--Se crea el campo mes con el formato MmmAAAA ejemplo: Ene2013 en la tabla de las ventas de Negocios Afiliados (no incluye Coppel)
if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_NegAfiliado') drop table tmp_VtasMensualMueblesFinal_NegAfiliado
select case
    when Mes=1 then 'Ene'+ cast(anio as char(4))
	when Mes=2 then 'Feb'+ cast(anio as char(4)) 
	when Mes=3 then 'Mar'+ cast(anio as char(4)) 
	when Mes=4 then 'Abr'+ cast(anio as char(4)) 
	when Mes=5 then 'May'+ cast(anio as char(4)) 
	when Mes=6 then 'Jun'+ cast(anio as char(4)) 
	when Mes=7 then 'Jul'+ cast(anio as char(4)) 
	when Mes=8 then 'Ago'+ cast(anio as char(4)) 
	when Mes=9 then 'Sep'+ cast(anio as char(4)) 
	when Mes=10 then 'Oct'+ cast(anio as char(4)) 
	when Mes=11 then 'Nov'+ cast(anio as char(4)) 
	when Mes=12 then 'Dic'+ cast(anio as char(4))
end Fecha,*
	into tmp_VtasMensualMueblesFinal_NegAfiliado
	from tmp_VtasMensualMuebles_NegAfiliado


--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal

--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal_Coppel

--select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas 
--from tmp_VtasMensualMueblesFinal_NegAfiliado

--Hola
--Como estás