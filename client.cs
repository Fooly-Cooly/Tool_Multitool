$remapDivision[$remapCount] = "MultiTool";
$remapName[$remapCount] = "Enable Multitool";
$remapCmd[$remapCount] = "EnableMulti";
$remapCount++;
$remapName[$remapCount] = "Next Mode";
$remapCmd[$remapCount] = "NextMode";
$remapCount++;
$remapName[$remapCount] = "Previous Mode";
$remapCmd[$remapCount] = "PreviousMode";
$remapCount++;

function EnableMulti(%val)
{
	if(%val)
		commandtoServer('EnableMulti');
}

function NextMode(%val)
{
	if(%val)
		commandtoServer('MM',"N");
}

function PreviousMode(%val)
{
	if(%val)
		commandtoServer('MM',"P");
}