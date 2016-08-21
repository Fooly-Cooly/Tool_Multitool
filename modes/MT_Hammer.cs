MT_AddMode("Hammer", $TypeMasks::FxBrickAlwaysObjectType, "MT_Hammer");

function MT_Hammer(%cl)
{
	hammerimage::onFire(hammerimage, %cl.player);
}