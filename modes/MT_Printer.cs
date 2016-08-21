MT_AddMode("Printer", $TypeMasks::FxBrickAlwaysObjectType, "MT_Printer");

function MT_Printer(%cl)
{
	printGunImage::onFire(printGunImage, %cl.player);
}
