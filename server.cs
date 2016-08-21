if(!isObject(MT_Modes))
		new ScriptObject(MT_Modes){};

function MT_AddMode(%name, %mask, %func)
{
	if(MT_Modes.idx_[%name] !$= "")
		return echo("\c2Multitool-Mode already added...");
	MT_Modes.count++;
	MT_Modes.idx_[%name] = MT_Modes.count;
	MT_Modes.mode[MT_Modes.count] = %name;
	MT_Modes.mask[MT_Modes.count] = %mask;
	MT_Modes.func[MT_Modes.count] = %func;
}

function serverCmdMT(%cl, %arg)
{
	switch$(%arg)
	{
		case "N":
					%cl.MultiTool++;
					%cl.MultiTool = (%cl.MultiTool > MT_Modes.count) ? 1 : %cl.MultiTool;
					messageClient(%cl, '', '\c2 %1.%2 Mode', %cl.MultiTool, MT_Modes.mode[%cl.MultiTool]);
		case "P":
					%cl.MultiTool--;
					%cl.MultiTool = (%cl.MultiTool < 1) ? MT_Modes.count : %cl.MultiTool;
					messageClient(%cl, '', '\c2 %1.%2 Mode', %cl.MultiTool, MT_Modes.mode[%cl.MultiTool]);
		case "spawn":
					%cl.MT_Auto = %cl.MT_Auto ? 0 : 1;
					%msg = %cl.MT_Auto ? "Spawn with Multitools: Off" : "Spawn with multitools: On";
					centerPrint(%cl, %msg, 3);
		default:
					if(%cl.minigame > 0)
						return;
					%item = %arg $= "gun" ? MTGunItem.getID() : MTWandItem.getID();
					%image = %item.image;
					%pl = %cl.player;
					%pl.updateArm(%image);
					%pl.mountimage(%image, 0);
					for(%i=0;%i<5;%i++)
					{
						if(%pl.tool[%i]) continue;
						%pl.tool[%i] = %item;
						messageClient(%cl, 'MsgItemPickup', '', %i, %item);
					}
	}
}

package AddMultiTool
{
	function onMissionLoaded()
	{
		MissionCleanup.add(MT_Modes);
		parent::onMissionLoaded();
	}

	function GameConnection::spawnPlayer(%this)
	{
		parent::spawnPlayer(%this);
		if(!%this.MT_Auto || isObject(%this.minigame))
			return;
		%pl = %this.player;
		for(%i=0;%i<5;%i++)
		{
			%pl.tool[%i] = 0;
			messageClient(%this, 'MsgItemPickup', '', %i);
		}
		%pl.tool[0] = MTGunItem.getID();
		messageClient(%this,'MsgItemPickup','',0,MMGunImage.getID());
		%pl.tool[1] = MTWandItem.getID();
		messageClient(%this,'MsgItemPickup','',1,MMWandItem.getID());
	}

	function servercmdLight(%cl)
	{
		%tool = %cl.player.getMountedImage(0).getName();
		if(%tool $= "MTWandImage" || %tool $= "MTGunImage")
			serverCmdMT(%cl, "N");
		else parent::servercmdLight(%cl);
	}
};
ActivatePackage(AddMultiTool);

exec("./datablocks.cs");
exec("./modes/MT_Hammer.cs");
exec("./modes/MT_Wrench.cs");
exec("./modes/MT_Printer.cs");
exec("./modes/MT_PrintFill.cs");
exec("./modes/MT_FillCan.cs");
exec("./modes/MT_BrickEraser.cs");
exec("./modes/MT_BrickKiller.cs");
exec("./modes/MT_MassDeleter.cs");
exec("./modes/MT_ObjectInfo.cs");
exec("./modes/MT_Measurer.cs");
exec("./modes/MT_Duplicator.cs");
