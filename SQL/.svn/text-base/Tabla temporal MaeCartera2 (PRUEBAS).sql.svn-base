use Carteras
go

--Se declara la variable fecha donde se almacenara la fecha del catFechas
declare @Fecha as smalldatetime
select @Fecha=Fecha from ODS.Dic2013.dbo.CatFechas

select month(FechaVenta)Mes,year(FechaVenta)Anio,TipoProducto,
sum(cast(isnull(SaldoALaFecha,0) as bigint))Saldo,
sum(cast(ltrim(rtrim(substring(dbo.fngeVencidoMuebles(@fecha,FechaVenta,ImporteVenta,InteresSobreCompra,PlazoVenta,AbonoMensual,SaldoALAFecha,Enganche),34,11))) as float))Vencido,
count(*)NumCuentas
--into VentasMuebles.dbo.MaeCarteraMuebles2
from ODS.Dic2013.dbo.MaeCarteraMuebles
group by year(FechaVenta),month(FechaVenta),TipoProducto
order by anio desc, mes desc
--Duración de 22:59 en el ODS
--Se espera una duración de 16:00 en el servidor de operacions