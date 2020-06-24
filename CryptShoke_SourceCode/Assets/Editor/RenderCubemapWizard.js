class RenderCubemapWizard extends ScriptableWizard
{
    var renderFromPosition : Transform;
    var cubemap : Cubemap;
    
    function OnWizardUpdate()
    {
        helpString = "Select transform to render from and cubemap to render into";
        isValid = (renderFromPosition != null) && (cubemap != null);
    }
    
    function OnWizardCreate()
    {
        // create temporary camera for rendering
        var go = new GameObject( "CubemapCamera", Camera );
        //go.camera.cullingMask = ~(1<<8);
        go.camera.backgroundColor = Color.black;
        // place it on the object
        go.transform.position = renderFromPosition.position;
        if( renderFromPosition.renderer )
        	go.transform.position = renderFromPosition.renderer.bounds.center;
        go.transform.rotation = Quaternion.identity;

        // render into cubemap        
        go.camera.RenderToCubemap( cubemap );
        
        // destroy temporary camera
        DestroyImmediate( go );
    }
    
    @MenuItem("Custom/Render into Cubemap", false, 4)
    static function RenderCubemap()
    {
        ScriptableWizard.DisplayWizard(
            "Render cubemap", RenderCubemapWizard, "Render!");
    }
}
