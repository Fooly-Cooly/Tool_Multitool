function MT_GetBrightestColor()
{
	for(%i = 0; %i < 64; %i++)
	{
		%colori = getColorIdTable(%i);
		%dist = vectorDist(%colori, "1 1 1 1");
		if(%dist < %best || %best $= "")
		{
			%best = %dist;
			$HilightColor = %i;
		}
	}
}

MT_GetBrightestColor();
$MT::Duplicator::MaxBricks = 500;
$MT::Duplicator::AdminMaxBricks = -1;
$MT::Duplicator::Timeout = 5;
$MT::Duplicator::AdminTimeout = 1;
$MT::Duplicator::PlantErrorMsgs = true;
$MT::Duplicator::Item = true;
$MT::Duplicator::TooFar = true;
$MT::Duplicator::AlwaysContinue = true;
MT_AddMode("Duplicator", $TypeMasks::FxBrickAlwaysObjectType, "MT_Duplicator");


package Duplicator
{
	function serverCmdDuplicator(%cl,%a0)
	{
		%pl = %cl.player;
		if(!isObject(%pl))
			return;
		switch$(%a0)
		{
			case "invert":	%cl.dupMode = 4;
			case   "stop":	%cl.dupMode = 3;
			case  "print":	%cl.dupMode = 2; 
			case  "owner":	%cl.dupMode = 1;
				  default:	%cl.dupMode = 0;
		}
		%pl.updateArm("MTWandImage");
		%pl.mountimage("MTWandImage", 0);
		%cl.MultiTool = MT_Modes.chec["Duplicator"];
	}

	function MT_Duplicator(%cl, %col)
	{
		if(getTrustLevel(%cl.player, %col) < 2)
			return;
		%cl.dupReset();
		%cl.dupGrp = new ScriptObject() { count = 0; };
		%col.dupBricks(%cl, %cl.dupGrp);
	}

	function GameConnection::dupReset(%cl)
	{
		%grp = %cl.dupGrp;
		if(!isObject(%cl) || !isObject(%grp))
			return;
		if((%tmp = %cl.player.tempbrick))
			%tmp.delete();
		for(%i=0; %i<%grp.count; %i++)
		{
			%grp.brick[%i].dup = 0;
			%grp.brick[%i].setColorFX(%brk.oldFX);
		}
		%grp.delete();
		%cl.dupGrp = 0;
		bottomPrint(%cl, "<color:FF8888>Normal <color:FF0000>Mode", 2, 2);
	}

	function fxDtsBrick::dupBricks(%this, %cl, %grp)
	{
		%cnt = %grp.count;
		if(%this.dup || (getTrustLevel(%cl.player, %this) < 2 && !%cl.isLan()) || %cl.dupMode == 3)
			return;
		%mb = %cl.isAdmin == 1 ? $MT::Duplicator::AdminMaxBricks : $MT::Duplicator::MaxBricks;
		if(%cnt == %mb)
			return messageClient(%cl, 'PlantError_Limit', "Duplicator: Limit of" SPC %mb SPC "reached...");
		cancel(%cl.dupEvent);
		%grp.brick[%cnt] = %this;
		%this.dup = 1;
		%this.oldFX = %this.colorFXID;
		%this.oldColor = %this.colorID;
		%this.setColorFX(3);
		%this.setColor($HilightColor);
		%this.schedule(500, setColorFX, 4);
		%this.schedule(500, setColor, %this.oldColor);
		%grp.count++;
		%i = 0;
		while((%brk = %this.getUpBrick(%i)) && (%i++))
			if(!%brk.dup)
				%brk.schedule(50, "dupBricks", %cl, %grp);
		%i = 0;
		while(%cnt && (%brk = %this.getDownBrick(%i)) && (%i++))
			if(!%brk.dup)
				%brk.schedule(50, "dupBricks", %cl, %grp);
		%cl.dupEvent = %cl.player.schedule(500, "dupCreateTemp", %grp);
		bottomPrint(%cl, "<color:FF8888>Duplication<color:FF0000> Mode\n" @ %grp.count @ " Brick/s Selected", 0, 3);
	}

	function Player::dupCreateTemp(%this, %grp)
	{
		%tmp = %grp.brick[0];
		%this.tempBrick = new fxDtsBrick()
		{
			position		= %tmp.position;
			rotation		= %tmp.rotation;
			angleID			= %tmp.angleID;
			colorID			= %tmp.colorID;
			colorFXID		= %tmp.colorFXID;
			shapeFXID		= %tmp.shapeFXID;
			printID			= %tmp.printID;
			dataBlock		= %tmp.dataBlock;
			isBasePlate		= %tmp.isBasePlate();
			client			= %this.client;
		};
	}

	function serverCmdPlantBrick(%cl)
	{
		%grp = %cl.dupGrp;
		if(%grp)
		{
			%timeOut = %cl.isAdmin == 1 ? $MT::Duplicator::AdminTimeout : $MT::Duplicator::Timeout;
			%nDTime = %cl.lastDup + %timeOut;
			if($Sim::Time < %nDTime)
			{
				%left = mCeil(%nDTime - $Sim::Time);
				%msg = "Duplicator Timeout: " @ %timeOut @ " secs\n<color:FF8888>" @ %left @ " secs left";
				messageClient(%cl, 'MsgPlantError_Flood', %msg);
				return;
			}
			%tmp = %cl.player.tempBrick;
			%grp.sPos = %tmp.position;
			%grp.angleDif = %tmp.angleID - %grp.brick[0].angleId;
			%grp.anglePos = %grp.angleDif + (%grp.angleDif < 0 ? 4 : 0);
			%cl.lastDup	= $Sim::Time;
			%cl.undoStack.push(%newBrick @ "\tPLANT");
			%cl.PlayDupTick(0);
		}
		else Parent::serverCmdPlantBrick(%cl);		
	}
	
	function GameConnection::playDupTick(%cl, %idx)
	{
		%grp = %cl.dupGrp;
		%brk = %grp.brick[%idx];
		%chk = %cl.playDup(%brk);
		if(%chk)
			%idx++;
		if(%idx < %grp.count)
			%cl.schedule(0.90, "playDupTick", %idx);
		centerPrint(%cl, "<color:00FF00>" @ %idx @ "/" @ %grp.count @ " Duplicated Successfully", 3, 2);
	}

	function GameConnection::PlayDup(%cl,%brk)
	{
		%grp = %cl.dupGrp;
		%anlDif = %grp.angleDif;
		%sPos = %grp.sPos;
		%pos = vectorsub(%brk.position, %grp.brick[0].position);
		switch(%grp.anglePos)
		{
			case 1:	%pos = getWord(%pos, 1) SPC -getWord(%pos, 0) SPC getWord(%pos, 2);
			case 2:	%pos = -getWord(%pos, 0) SPC -getWord(%pos, 1) SPC getWord(%pos, 2);
			case 3: %pos = -getWord(%pos, 1) SPC getWord(%pos, 0) SPC getWord(%pos, 2);
		}
		%pos	= vectoradd(%pos, %sPos);
		%anl2	= dupRotate("", %brk.angleID, %anlDif);
		switch(%anl2)
		{
			case 0:%rot = "1 0 0 0";
			case 1:%rot = "0 0 1 90";
			case 2:%rot = "0 0 1 180";
			case 3:%rot = "0 0 -1 90";
		}
		%db = %brk.dataBlock.getID();
		switch(%cl.DupMode)
		{
			case 2:	%bCl = %cl;
					%db = strstr(%brk.Datablock,"brick1x1") > 0 ? brick1x1fPrintData.getID() : %db;
					if(%brk.Datablock $= "brick1x1Data")
						%pos = vectorsub(%pos,"0 0 0.2");
			case 1:	%bCl = -1;
					%bGrp = %brk.getGroup();
			default:%bCl = %cl;
					%bGrp = %bCl.brickGroup;
		}
		%nBrk = new fxDtsBrick()
		{
			position	= %pos;
			rotation	= %rot;
			angleID		= %anl2;
			colorID		= %brk.colorID;
			colorFXID	= %brk.OldColorFXID;
			shapeFXID	= %brk.ShapeFXID;
			printID		= %brk.printID;
			dataBlock	= %db;
			isBasePlate	= %brk.BasePlate;
			client		= %bCl;
			isPlanted	= 1;
		};
		%fail = %nBrk.plant();
		if(%fail)
		{
			%nBrk.delete();
			return 0;
		}
		else
		{
			if(%nBrk.isBasePlate())
				%cl.undoStack.push(%nBrk @ "\tPLANT");
			%bGrp.add(%nBrk);
			%nBrk.applySpecialAttributes(%brk, %anlDif);
			%nBrk.plantedTrustCheck();
			%nBrk.setTrusted(1);
			return 1;
		}
	}

	function fxDTSBrick::applySpecialAttributes(%nb, %ob, %anlDif)
	{
		%quota = getQuotaObjectFromBrick(%nb);
		%var = %ob.isRayCasting();
		%nb.setRayCasting(%var);
		%var = %ob.isColliding();
		%nb.setColliding(%var);
		%var = %ob.isRendering();
		%nb.setRendering(%var);
		%var = %ob.getName();
		if(%var !$= "")
			%nb.setNTObjectName(%var);
		if(%ob.light)
			%nb.setLight(%ob.light.getDatablock());
		if(%ob.emitter)
		{
			%var = %ob.emitterDirection;
			%var = %var > 1 ? dupRotate("",%var - 2,%anlDif) + 2 : %var;
			%nb.setEmitter(%ob.emitter.emitter.getID());
			%nb.setEmitterDirection(%var);
			%quota.Allocs_Environment--;
		}
		if(%ob.item)
		{
			%var = %ob.itemDirection;
			%var = %var > 1 ? dupRotate("",%var - 2,%anlDif) + 2 : %var;
			%nb.setItem(%ob.item.getDatablock());
			%nb.setItemDirection(%var);
			%var = %ob.itemPosition;
			%var = %var > 1 ? dupRotate("",%var - 2,%anlDif) + 2 : %var;
			%nb.setItemPosition(%var);
			%nb.setItemRespawnTime(%ob.itemRespawnTime);
		}
		%var = %ob.numEvents;
		for(%i=0; %i<%var; %i++)
		{
			%nb.numEvents					= %var;
			%nb.eventDelay[%i]				= %ob.eventDelay[%i];
			%nb.eventEnabled[%i]			= %ob.eventEnabled[%i];
			%nb.eventInput[%i]				= %ob.eventInput[%i];
			%nb.eventInputIdx[%i]			= %ob.eventInputIdx[%i];
			%nb.eventTarget[%i]				= %ob.eventTarget[%i];
			%nb.eventTargetIdx[%i]			= %ob.eventTargetIdx[%i];
			%nb.eventNT[%i]					= %ob.eventNT[%i];
			%nb.eventOutput[%i]				= dupRotate(%ob.eventOutput[%i],'',%anlDif);
			%nb.eventOutputAppendClient[%i] = %ob.eventOutputAppendClient[%i];
			%nb.eventOutputIdx[%i]			= %nb.eventOutput[%i] $= %ob.eventOutput[%i] ? %ob.eventOutputIdx[%i] : outputEvent_GetOutputEventIdx(fxDTSBrick,%nb.eventOutput[%i]);
			%nb.eventOutputParameter[%i,1]	= dupRotate(%ob.eventOutputParameter[%i,1],'',%anlDif);
			%nb.eventOutputParameter[%i,2]	= dupRotate(%ob.eventOutputParameter[%i,2],'',%anlDif);
			%nb.eventOutputParameter[%i,3]	= dupRotate(%ob.eventOutputParameter[%i,3],'',%anlDif);
			%nb.eventOutputParameter[%i,4]	= dupRotate(%ob.eventOutputParameter[%i,4],'',%anlDif);
		}
		%var = %ob.vehicleDataBlock;
		if(%var)
		{
			switch$(%var.getClassName())
			{
				case "PlayerData":
					if($Pref::Server::MaxPlayerVehicles_Total != $Server::numPlayerVehicles)
						%quota.Allocs_Player--;
				default:
					if($Pref::Server::MaxPhysVehicles_Total != $Server::numPhysVehicles)
						%quota.Allocs_Vehicle--;
			}
			%nb.setVehicle(%var);
			%nb.setReColorVehicle(%ob.recolorVehicle);
		}
		//%Group.brickdata[%Group.count,AudioEmitter]		= %ob.AudioEmitter.profile;
	}
	
	function GameConnection::OnClientLeaveGame(%this)
	{
		%this.dupReset();
		Parent::OnClientLeaveGame(%this);
	}
	
	function fxDtsBrickData::onRemove(%this, %obj)
	{
		%cl = %obj.client;
		if(%cl.dupGrp)
			%cl.dupReset();
		Parent::onRemove(%this, %obj);
	}

	function fxDtsBrick::setDataBlock(%this, %db)
	{
		%cl = %this.client;
		if(%cl.dupGrp && %db != %this.getDataBlock().getID())
			%cl.dupReset();
		Parent::setDataBlock(%this, %db);
	}

	function dupRotate(%event, %dir, %dif)
	{
		echo(%evnt," | ",%dir," | ",%dif);
		switch$(%event)
		{
			case "fire":
				%tokens = "North East South West";
				for(%i=0; %i<4; %i++)
					if((%word = getWord(%tokens, %i)) && strstr(%event, %word))
						break;
			case "":
			case "":
			case "":
			case "":
		}
		
		//Directions
		%dir	= %dir + %dif;
		%dir   += %dir < 0 ? 4 : 0;
		%dir   -= %dir > 3 ? 4 : 0;
		%evnt	= %oDir !$= "" ? strreplace(%evnt, %oDir, %dir[%dir]) : %evnt;
		%rtn	= %evnt !$= "" ? %evnt : %dir;
		//
		
		
		
		return %rtn;
	}

	function saveDupBricks(%cl) {
		%cl = findclientbyname(%cl);
		%grp = "Duplicator_" @ %cl.bl_id;
		%name = "test";
		%File = new FileObject();
		%File.openForWrite("saves/Duplicator/"@%name@".cs");

		%File.writeLine(%grp.count);
		for(%i=0;%i<%grp.count;%i++) {
			%brk = %grp.brick[%i];
			%line = %brk.light.getDatablock() TAB 
			%File.writeLine(%brk.position);
			%File.writeLine(%brk.angleID);
			%File.writeLine(%brk.colorID);
			%File.writeLine(%brk.OldColorFXID);
			%File.writeLine(%brk.ShapeFXID);
			%File.writeLine(%brk.printID);
			%File.writeLine(%brk.Datablock);
			%File.writeLine(%brk.BasePlate);
			%File.writeLine(%brk.isRayCasting());
			%File.writeLine(%brk.isColliding());
			%File.writeLine(%brk.isRendering());
			%File.writeLine(%brk.getName());
			//%File.writeLine();
			%File.writeLine(%brk.BasePlate);
			//--emitter--------------------------------
			%File.writeLine(%brk.emitter);
			%File.writeLine(%brk.emitterDirection);
			//--item-----------------------------------
			%File.writeLine(%brk.item);
			%File.writeLine(%brk.itemDirection);
			%File.writeLine(%brk.itemPosition);
			%File.writeLine(%brk.itemRespawnTime);
			%var = 0;//%brk.numEvents;
			%File.writeLine(%var);
		for(%j=0;%j<%var;%j++) {
			%File.writeLine(%var);
			%File.writeLine(%brk.eventDelay[%j]);
			%File.writeLine(%brk.eventEnabled[%j]);
			%File.writeLine(%brk.eventInput[%j]);
			%File.writeLine(%brk.eventInputIdx[%j]);
			%File.writeLine(%brk.eventTarget[%j]);
			%File.writeLine(%brk.eventTargetIdx[%j]);
			%File.writeLine(%brk.eventNT[%j]);
			%File.writeLine(%brk.eventOutput[%j]);
			%File.writeLine(%brk.eventOutputAppendClient[%j]);
			%File.writeLine(%brk.eventOutputIdx[%j]);
			%File.writeLine(%brk.eventOutputParameter[%j,1]);
			%File.writeLine(%brk.eventOutputParameter[%j,2]);
			%File.writeLine(%brk.eventOutputParameter[%j,3]);
			%File.writeLine(%brk.eventOutputParameter[%j,4]);
		}

		//%var = %ob.vehicleDataBlock;
		//if(%var) {
		//	switch$(%var.getClassName()) {
		//		case "PlayerData":
		//			if($Pref::Server::MaxPlayerVehicles_Total != $Server::numPlayerVehicles) {
		//				%quota.Allocs_Player--;
		//			}
		//		default:if($Pref::Server::MaxPhysVehicles_Total != $Server::numPhysVehicles) {
		//				%quota.Allocs_Vehicle--;
		//			}
		//	}
		//	%nb.setVehicle(%var);
		//	%nb.setReColorVehicle(%ob.recolorVehicle);
		//}
		}
		%File.close();
		%File.delete();
	}

	function serverCmdloadDupBricks(%cl,%fName) {
		%fname = "test";
		%File = new FileObject();
		%File.openForRead("saves/Duplicator/"@%fName@".cs");

		%grp = "Duplicator_" @ %cl.bl_id;
		new ScriptObject(%grp) {
			class = "DuplicatorObjSO";
			count = %File.ReadLine();
		};
		for(%i=0;%i<%grp.count;%i++) {
			%grp.brick[%i] = new ScriptObject() {
				class = "DupfxDTSBrick";
				position	= %File.ReadLine();
				angleID		= %File.ReadLine();
				colorID		= %File.ReadLine();
				OldColorFXID	= %File.ReadLine();
				ShapeFXID	= %File.ReadLine();
				printID		= %File.ReadLine();
				Datablock	= %File.ReadLine();
				BasePlate	= %File.ReadLine();
				isRayCasting	= %File.ReadLine();
				isColliding	= %File.ReadLine();
				isRendering	= %File.ReadLine();
				name		= %File.ReadLine();
				//%File.ReadLine(light.getDatablock());
				BasePlate	= %File.ReadLine();
				//--emitter------------------------------------------------
				emitter		= %File.ReadLine();
				emitterDirection= %File.ReadLine();
				//--item---------------------------------------------------
				item		= %File.ReadLine();
				itemDirection	= %File.ReadLine();
				itemPosition	= %File.ReadLine();
				itemRespawnTime	= %File.ReadLine();
				numEvents	= %File.ReadLine();
			};
			//--events-------------------------------------------------
		for(%j=0;%j<%grp.brick[%i].numEvents;%j++) {
			%grp.brick[%i].eventDelay[%j]			= %File.ReadLine();
			%grp.brick[%i].eventEnabled[%j]			= %File.ReadLine();
			%grp.brick[%i].eventInput[%j]			= %File.ReadLine();
			%grp.brick[%i].eventInputIdx[%j]		= %File.ReadLine();
			%grp.brick[%i].eventTarget[%j]			= %File.ReadLine();
			%grp.brick[%i].eventTargetIdx[%j]		= %File.ReadLine();
			%grp.brick[%i].eventNT[%j]			= %File.ReadLine();
			%grp.brick[%i].eventOutput[%j]			= %File.ReadLine();
			%grp.brick[%i].eventOutputAppendClient[%j]	= %File.ReadLine();
			%grp.brick[%i].eventOutputIdx[%j]		= %File.ReadLine();
			%grp.brick[%i].eventOutputParameter[%j,1]	= %File.ReadLine();
			%grp.brick[%i].eventOutputParameter[%j,2]	= %File.ReadLine();
			%grp.brick[%i].eventOutputParameter[%j,3]	= %File.ReadLine();
			%grp.brick[%i].eventOutputParameter[%j,4]	= %File.ReadLine();
		}


		//%var = %ob.vehicleDataBlock;
		//if(%var) {
		//	switch$(%var.getClassName()) {
		//		case "PlayerData":
		//			if($Pref::Server::MaxPlayerVehicles_Total != $Server::numPlayerVehicles) {
		//				%quota.Allocs_Player--;
		//			}
		//		default:if($Pref::Server::MaxPhysVehicles_Total != $Server::numPhysVehicles) {
		//				%quota.Allocs_Vehicle--;
		//			}
		//	}
		//	%nb.setVehicle(%var);
		//	%nb.setReColorVehicle(%ob.recolorVehicle);
		//}
		}
		%File.close();
		%File.delete();
	}
};
ActivatePackage(Duplicator);