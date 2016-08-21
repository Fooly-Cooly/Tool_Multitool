$MT::FillCan::Limit = 500;
$MT::FillCan::SuperLimit = -1;
MT_AddMode("Fill-Can", $TypeMasks::FxBrickAlwaysObjectType, "MT_FillCan");

package FillCan
{
	function MT_FillCan(%cl, %col)
	{
		if(%col.getGroup().bl_id != %cl.bl_id)
				return centerPrint(%cl, "You don't own this brick!", 1);
		%pl = %cl.player;
		%lst = new scriptobject() { count = 0; };
		%lst.effect = %pl.currFXCan > -1 ? 1 : 0;
		%lst.effect = %pl.currShapeCan > -1 ? 2 : %lst.effect;
		%lst.spray0 = %lst.effect == 1 ? %pl.currFXCan : %pl.currSprayCan;
		%lst.spray0 = %lst.effect == 2 ? %pl.currShapeCan : %lst.spray0;
		switch(%lst.effect)
		{
			case 0: %lst.spray1 = %col.getColorID();
			case 1: %lst.spray1 = %col.colorFxID;
			case 2: %lst.spray1 = %col.ShapeFXID;
		}
		if(%lst.spray0 != %lst.spray1)
		{
			missionCleanup.add(%lst);
			%cl.undoStack.push(%lst TAB "FILLCAN");
			MT_SetColor(%cl, %col, %lst);
		}
		else %lst.delete();
	}
	
	function MT_SetColor(%cl, %col, %lst)
	{
		if(!isObject(%col) || !isObject(%lst)) return;
		%colDB = %col.getDataBlock();
		%rot = round(getword(%col.rotation,3));
		%limit = %cl.isSuperAdmin ? $MT::FillCan::SuperLimit : $MT::FillCan::Limit;
		%area = %rot == 90 ? %colDB.brickSizeY SPC %colDB.brickSizeX : %colDB.brickSizeX SPC %colDB.brickSizeY;
		%area = %area SPC %colDB.brickSizeZ ;
		%area = vectorScale(%area, "0.55 0.55 0.25");
		InitContainerBoxSearch(%col.getposition(), %area, $TypeMasks::FxBrickAlwaysObjectType);
		while((%brk = containerSearchNext()) && %lst.count != %limit)
		{
			if(%brk.getGroup().bl_id != %cl.bl_id)
			{
				centerPrint(%cl, "You don't own this brick!", 1);
				continue;
			}
			switch(%lst.effect)
			{
				case 0: if(%lst.spray1 != %brk.getColorID())
							continue;
						%brk.setColor(%lst.spray0);
				case 1: if(%lst.spray1 != %brk.colorFxID)
							continue;
						%brk.setColorFx(%lst.spray0);
				case 2: if(%lst.spray1 != %brk.ShapeFXID)
							continue;
						%brk.setShapeFx(%lst.spray0);
			}
			%lst.brick[%lst.count] = %brk;
			%lst.count++;
			bottomPrint(%cl, "\c2Bricks Filled:\c4" SPC %lst.count, 2);
			schedule(200, 0, "MT_SetColor", %cl, %brk, %lst);
		}
	}

	function serverCmdFillCan(%cl)
	{
		%pl = %cl.player;
		if(%cl.minigame > 0 || !isObject(%pl))
			return;
		%pl.updateArm("MTWandImage");
		%pl.mountimage("MTWandImage",0);
		%cl.MultiTool = MT_Modes.chec["Fill-Can"];
	}

	function servercmdUseSprayCan(%cl, %color)
	{
		Parent::serverCmdUseSprayCan(%cl, %color);
		%cl.player.currShapeCan = -1;
		if(%cl.MultiTool != MT_Modes.chec["Fill-Can"])
			return;
		%cl.Player.updateArm("MTWandImage");
		%cl.Player.mountimage("MTWandImage",0);
	}

	function servercmdUseFxCan(%cl, %fx)
	{
		Parent::serverCmdUseFxCan(%cl, %fx);	
		%cl.player.currShapeCan = %fx > 6 ? %fx - 7 : -1;
		if(%cl.MultiTool != MT_Modes.chec["Fill-Can"])
			return;
		%cl.player.updateArm("MTWandImage");
		%cl.player.mountimage("MTWandImage",0);
	}
	
	function serverCmdUndoBrick(%cl)
	{
		%str = %cl.undostack.val[%cl.undostack.head-1];
		if(getfield(%str, 1) $= "FILLCAN")
		{
			%lst = getfield(%str, 0);
			while(%brk = %lst.brick[%lst.count--])
				if(isobject(%brk))
					switch(%lst.effect)
					{
						case 0: %brk.setColor(%lst.spray1);
						case 1: %brk.setColorFX(%lst.spray1);
						case 2: %brk.setShapeFX(%lst.spray1);
					}
			%lst.delete();
			%cl.undostack.pop();
			%cl.player.playthread(0, undo);
		}
		else parent::serverCmdUndoBrick(%cl);
	}

	function GameConnection::onClientLeaveGame(%cl)
	{
		while(%str = %cl.undoStack.val[%cl.undoStack.head--])
			if(getword(%str, 1) $= "FILLCAN") getword(%str, 0).delete();
		parent::onClientLeaveGame(%cl);
	}
};
activatePackage(FillCan);