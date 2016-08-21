$MT::PrintFill::MaxBricks = 500;
$MT::PrintFill::MaxSuperBricks = -1;
MT_AddMode("Print-Fill", $TypeMasks::FxBrickAlwaysObjectType, "MT_PrintFill");

package PrintFill
{
	function MT_PrintFill(%cl, %col)
	{
		%ar = %col.getDataBlock().printAspectRatio;
		if(%col.getGroup().bl_id != %cl.bl_id)
			centerPrint(%cl, "You don't own this brick!", 1);
		else if(%ar)
		{
			%cl.printBrick = %col;
			commandToClient(%cl, 'openPrintSelectorDlg', %ar, $printARStart[%ar], $printARNumPrints[%ar]);
		}
	}

	function MT_CreatePrintUndo(%cl, %prt)
	{
		%col = %cl.printBrick;
		%lst = new scriptobject() { count = 0; };
		if(%col.printID != %prt)
		{
			%lst.nPrint = %prt;
			%lst.oPrint = %col.printID;
			missionCleanup.add(%lst);
			%cl.undoStack.push(%lst TAB "PRINTFILL");
			MT_SetPrint(%cl, %col, %lst);
		}
		else %lst.delete();
	}

	function MT_SetPrint(%cl, %col, %lst)
	{
		if(!isObject(%col) || !isObject(%lst)) return;
		%colDB = %col.getDataBlock();
		%rot = round(getword(%col.rotation,3));
		%area = %rot == 90 ? %colDB.brickSizeY SPC %colDB.brickSizeX : %colDB.brickSizeX SPC %colDB.brickSizeY;
		%area = %area SPC %colDB.brickSizeZ;
		%area = vectorScale(%area, "0.55 0.55 0.25");
		InitContainerBoxSearch(%col.getPosition(), %area, $TypeMasks::FxBrickAlwaysobjecttype);
		while((%brk = containerSearchNext()) && %lst.count < $MT::PrintFill::MaxBricks)
		{
			if(%brk.getGroup().bl_id != %cl.bl_id)
				centerPrint(%cl, "You don't own this brick!", 1);
			else if(%brk.getDataBlock().printAspectRatio && %brk.printID == %lst.oPrint)
			{
				%brk.setPrint(%lst.nPrint);
				%lst.brick[%lst.count] = %brk;
				%lst.count++;
				bottomPrint(%cl, "\c2Bricks Printed:\c4" SPC %lst.count, 2);
				schedule(200, 0, "MT_SetPrint", %cl, %brk, %lst);
			}
		}
	}
	
	function serverCmdSetPrint(%cl, %prt)
	{
		if(%cl.MultiTool == MT_Modes.chec["Print-Fill"]) MT_CreatePrintUndo(%cl, %prt);
		else parent::serverCmdSetPrint(%cl, %prt);
	}
	
	function serverCmdUndoBrick(%cl)
	{
		%str = %cl.undostack.val[%cl.undostack.head-1];
		if(getfield(%str, 1) $= "PRINTFILL")
		{
			%lst = getfield(%str, 0);
			while(%brk = %lst.brick[%lst.count--])
				if(isobject(%brk))
					%brk.setPrint(%lst.oPrint);
			%lst.delete();
			%cl.undostack.pop();
			%cl.player.playthread(0, undo);
		}
		else parent::serverCmdUndoBrick(%cl);
	}
};
ActivatePackage(PrintFill);