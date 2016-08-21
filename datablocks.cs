datablock AudioProfile(MultiToolFireSound)
{
	filename	= "base/data/sound/bodyRemove.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock ParticleData(MultiToolSmokeParticle)
{
	dragCoefficient			= 3;
	gravityCoefficient		= -0.5;
	inheritedVelFactor		= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS				= 525;
	lifetimeVarianceMS		= 55;
	textureName				= "base/data/particles/cloud";
	spinSpeed				= 10.0;
	spinRandomMin			= -500.0;
	spinRandomMax			= 500.0;

	colors[0]				= "0.5 0.5 0.5 0.9";
	colors[1]				= "0.5 0.5 0.5 0.0";

	sizes[0]				= 0.15;
	sizes[1]				= 0.15;

	useInvAlpha				= false;
};
datablock ParticleEmitterData(MultiToolSmokeEmitter)
{
	ejectionPeriodMS	= 3;
	periodVarianceMS	= 0;
	ejectionVelocity	= 1.0;
	velocityVariance	= 1.0;
	ejectionOffset		= 0.0;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "MultiToolSmokeParticle";
};
datablock ParticleData(MultiToolFlashParticle)
{
	dragCoefficient			= 3;
	gravityCoefficient		= -0.5;
	inheritedVelFactor		= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS				= 25;
	lifetimeVarianceMS		= 15;
	textureName				= "base/data/particles/star1";
	spinSpeed				= 10.0;
	spinRandomMin			= -500.0;
	spinRandomMax			= 500.0;

	colors[0]				= "0.0 0.0 1.0 0.9";
	colors[1]				= "0.0 1.0 0.0";

	sizes[0]				= 0.5;
	sizes[1]				= 1.0;

	useInvAlpha				= false;
};
datablock ParticleEmitterData(MultiToolFlashEmitter)
{
	ejectionPeriodMS	= 3;
	periodVarianceMS	= 0;
	ejectionVelocity	= 1.0;
	velocityVariance	= 1.0;
	ejectionOffset		= 0.0;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "MultiToolFlashParticle";
};

datablock ParticleData(MultiToolTrailParticle)
{
	dragCoefficient			= 3.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 0.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 200;
	lifetimeVarianceMS		= 0;
	spinSpeed				= 10.0;
	spinRandomMin			= -50.0;
	spinRandomMax			= 50.0;
	useInvAlpha				= false;
	animateTexture			= false;
	textureName				= "base/data/particles/thinring";

	colors[0]				= "0.0 1.0 0.0";
	colors[1]				= "0.0 0.0 1.0";

	sizes[0]				= 0.2;
	sizes[1]				= 0.4;

	times[0]				= 0.0;
	times[1]				= 1.0;
};

datablock ParticleEmitterData(MultiToolTrailEmitter)
{
	ejectionPeriodMS	= 2;
	periodVarianceMS	= 0;
	ejectionVelocity	= 0;
	velocityVariance	= 0;
	ejectionOffset		= 0;
	thetaMin			= 0.0;
	thetaMax			= 90.0;
	particles			= MultiToolTrailParticle;
};

datablock ProjectileData(MTProjectile)
{
	directDamage		= 0;
	directDamageType	= $DamageType::Gun;
	radiusDamage		= 0;
	damageRadius		= 0;
	radiusDamageType	= $DamageType::Gun;
	explosion			= spawnExplosion;
	particleEmitter		= MultiToolTrailEmitter;
	muzzleVelocity		= 65;
	velInheritFactor	= 1;
	armingDelay			= 0;
	lifetime			= 4000;
	fadeDelay			= 3500;
	bounceElasticity	= 0;
	bounceFriction		= 0;
	isBallistic			= true;
	gravityMod			= 0.0;
	hasLight			= true;
	lightRadius			= 3.0;
	lightColor			= "0 0 0.5";
};

datablock ItemData(MTWandItem : WandItem)
{
	uiName			= "Multitool Wand";
	doColorShift	= true;
	colorShiftColor	= "0 0.710 0.250 1";
	image			= MTWandImage;
	canDrop			= true;
};

datablock ShapeBaseImageData(MTWandImage : WandImage)
{
	item = MTWandItem;

	projectile						= "";
	projectileType					= "";
	doColorShift					= true;
	colorShiftColor					= MTWandItem.colorShiftColor;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.01;
	stateTransitionOnTimeout[0]		= "Ready";
	stateEmitter[0]					= MultiToolTrailEmitter;
	stateEmitterTime[0]				= 1000;
	stateSound[0]					= weaponSwitchSound;

	stateName[1]					= "Ready";
	stateEmitter[1]					= MultiToolTrailEmitter;
	stateEmitterTime[1]				= 1000;
	stateTransitionOnTriggerDown[1]	= "PreFire";
	stateAllowImageChange[1]		= true;
};

//Gun Multitool
datablock ItemData(MTGunItem)
{
	category			= "Tools";
	className			= "Tool";
	shapeFile			= "Add-Ons/Weapon_Gun/pistol.dts";
	rotate				= false;
	mass					= 1;
	density				= 0.2;
	elasticity			= 0.2;
	friction			= 0.6;
	emap					= true;
	uiName				= "MultiTool Gun";
	iconName			= "Add-Ons/Weapon_Gun/Icon_gun";
	doColorShift	= true;
	colorShiftColor		= MTWandItem.colorShiftColor;
	image							= MTGunImage;
	canDrop						= true;
};

datablock ShapeBaseImageData(MTGunImage)
{
	shapeFile			="Add-Ons/Weapon_Gun/pistol.dts";
	emap				= true;
	mountPoint			= 0;
	offset				= "0 0 0";
	eyeOffset			= 0;
	rotation			= eulerToMatrix( "0 0 10" );
	correctMuzzleVector	= true;
	className			= "WeaponImage";
	item				= MTGunItem;
	ammo				= "";
	projectile			= MTProjectile;
	projectileType		= Projectile;
	shellExitDir		= "1.0 -1.3 1.0";
	shellExitOffset		= "0 0 0";
	shellExitVariance	= 15.0;
	shellVelocity		= 7.0;
	melee				= false;
	armReady			= true;
	doColorShift		= true;
	colorShiftColor		= MTWandItem.colorShiftColor;

	stateName[0]				= "Activate";
	stateSound[0]				= weaponSwitchSound;
	stateTimeoutValue[0]		= 0.15;
	stateTransitionOnTimeout[0]	= "Ready";

	stateName[1]					= "Ready";
	stateSequence[1]				= "Ready";
	stateAllowImageChange[1]		= true;
	stateTransitionOnTriggerDown[1]	= "Fire";

	stateName[2]				= "Fire";
	stateSequence[2]			= "Fire";
	stateScript[2]				= "onFire";
	stateFire[2]				= true;
	stateEjectShell[2]			= true;
	stateWaitForTimeout[2]		= true;
	stateAllowImageChange[2]	= false;
	stateSound[2]				= MultiToolFireSound;
	stateEmitter[2]				= MultiToolFlashEmitter;
	stateEmitterTime[2]			= 0.05;
	stateEmitterNode[2]			= "muzzleNode";
	stateTimeoutValue[2]		= 0.14;
	stateTransitionOnTimeout[2]	= "Smoke";

	stateName[3]					= "Smoke";
	stateSequence[3]				= "Reload";
	stateEmitter[3]					= MultiToolSmokeEmitter;
	stateEmitterTime[3]				= 0.05;
	stateEmitterNode[3]				= "muzzleNode";
	stateTransitionOnTriggerUp[3]	= "Ready";
};

function MTWandImage::onPreFire(%this, %obj, %slot){%obj.playthread(2, armattack);}
function MTWandImage::onStopFire(%this, %obj, %slot){%obj.playthread(2,root);}
function MTWandImage::onFire(%this,%obj,%slot)
{
	%cl = %obj.client;
	%eye = %obj.getEyePoint();
	%vec = %obj.getEyeVector();
	%obj.playThread(2, shiftAway);
	%end = vectorAdd(%eye, vectorScale(%vec,10));
	%targ = ContainerRayCast(%eye, %end, MT_Modes.mask[%cl.MultiTool], %obj);
	if(isObject(%targ))
	{
		%P = new Projectile()
		{
			dataBlock		= MTProjectile;
			initialVelocity = vectorScale(%vec, MTGunProjectile.muzzleVelocity);
			initialPosition = getWords(%targ, 1, 3);
			sourceObject    = %obj;
			sourceSlot      = 0;
			client          = %cl;
		};
		%p.explode();
		call(MT_Modes.func[%cl.MultiTool], %cl, %targ);
	}
}

function MTProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	%col = %col SPC %fade SPC %pos SPC %normal;
	call(MT_Modes.func[%cl.MultiTool], %obj.client, %col);
}
