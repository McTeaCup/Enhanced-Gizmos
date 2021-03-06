//Created by Daniel Redelius

using UnityEditor;
using UnityEngine;

public static class EnhancedGizmos
{
    //Not supposed to be visible for the users, only to streamline development

    #region Shape Source Code

    //Draw Cube
    static void Cube(Vector3 position, Vector3 size, Color color, float alpha, bool showText)
    {
        bool isSquare;
        bool isCube;
        float squareSize;

        //Is the given measurement cubic?
        isCube = size.x == size.y && size.x == size.z ? true : false;

        //Is the given measurement square?
        isSquare = size.x == size.y && size.z == 0 ||
                   size.x == size.z && size.y == 0 ||
                   size.y == size.z && size.x == 0;

        #region Information Text

        if (showText)
        {
            //If Square, Display "Xm^2 (X,Y,Z)"
            if (isSquare)
            {
                squareSize = size.x > 0 ? size.x : size.y;
                DrawInfoText(position + new Vector3(0, 0.2f, 0),
                    $"Pos: {position.ToString("F1")}\nSize: ({squareSize.ToString("F1")}m^2)");
            }

            //If Cube, Display "Xm^3 (X,Y,Z)"
            else if (isCube)
            {
                DrawInfoText(position + new Vector3(0, size.y / 1.2f, 0),
                    $"Pos: {position.ToString("F1")}\nSize: ({size.y.ToString("F1")}m^3)");
            }

            //If any other form, Display "(X,Y,Z)"
            else
            {
                DrawInfoText(position + new Vector3(0, size.y / 1.2f, 0),
                    $"Pos: {position.ToString("F1")}\nSize: {size.ToString("F1")}");
            }
        }

            #endregion

            Gizmos.color = color;
            Gizmos.DrawWireCube(position, size);
            Gizmos.color = new Color(color.r, color.g, color.b, alpha);
            Gizmos.DrawCube(position, size);
    }

    //Draw Sphere
    static void Sphere(Vector3 position, float radius, Color color, float alpha, bool showText)
    {
        if (showText)
        {
            DrawInfoText(position + new Vector3(0, radius + .3f, 0),
                $"r = {radius}\n {position.ToString("F1")}");
        }

        Gizmos.color = new Color(color.r, color.g, color.b, alpha);
        Gizmos.DrawSphere(position, radius);
        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        Gizmos.DrawWireSphere(position, Mathf.Abs(radius));
    }

    //Draw Mesh
    static void Mesh(Mesh mesh, Vector3 objectPosition, Quaternion objectRotation, Vector3 objecScale,
        Color color, float alpha, bool showText)
    {
        if (showText)
        {
            DrawInfoText(objectPosition + new Vector3(0, 0.7f, 0),
                $"Vertices: {mesh.vertexCount.ToString()}" +
                $"\nPos: {objectPosition.ToString("F1")}" +
                $"\nSize: {objecScale.ToString("F1")}");
        }

        GetHoloGizmosMesh(mesh, objectPosition, objectRotation, objecScale, color, alpha);
    }

    //Draw Arc
    static void Arc(Vector3 arcCenter, Vector3 arcUpVector, Vector3 startPoint, float angle, float radius,
        Color color, float alpha, bool showText)
    {
        if (angle > 0)
        {
            if (showText)
            {
                //If the angle is lower than 1 rotation
                if (angle < 360.01f)
                {
                    Handles.Label(arcCenter + startPoint * radius, $"{angle}??\nr = {radius}", new GUIStyle("Box"));
                }
                //If the angle is higher, display how many rotations
                else
                {
                    Handles.Label(arcCenter + startPoint * radius,
                        $"{(int) angle / 360}x {angle - (360 * ((int) angle / 360))}??\nr = {radius}",
                        new GUIStyle("Box"));
                }
            }

            Handles.color = new Color(color.r, color.g, color.b, 1f);
            Handles.DrawDottedLine(arcCenter, arcCenter + startPoint * radius, 3);

            float rad = Mathf.Deg2Rad * (-angle + 90);
            Vector3 endPoint = arcCenter;
            endPoint += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Handles.DrawLine(arcCenter, endPoint);

            Handles.color = new Color(color.r, color.g, color.b, alpha);
            Handles.DrawSolidArc(arcCenter, arcUpVector, startPoint, angle, radius);
            Handles.color = new Color(color.r, color.g, color.b, 1f);
            Handles.DrawWireArc(arcCenter, arcUpVector, startPoint, angle, radius);
        }
        else
        {
            Handles.Label(arcCenter + startPoint * radius, $"The angle is too small ({angle}??)", new GUIStyle("Box"));
        }
    }

    //Draw View Arc
    static void ViewArc(Vector3 arcCenter, Vector3 arcForwardVector, float angle,
        float radius, Color color, float alpha, bool showText)
    {
        if (angle > 0 && angle < 360.01f)
        {
            if (showText)
            {
                //If the angle is higher, display how many rotations
                DrawInfoText(arcCenter + arcForwardVector * radius, $"{angle}??\nr = {radius}");
            }
            Handles.color = new Color(color.r, color.g, color.b, 1f);

            Vector3 arcUpVector = arcForwardVector == Vector3.forward ? Vector3.up : Vector3.forward;

            //Draws the edgeline for each edge 
            if (angle < 360)
            {
                float rad1 = Mathf.Deg2Rad * (-angle * 0.5f + 90);
                float rad2 = Mathf.Deg2Rad * (angle * 0.5f + 90);
                Vector3 endPoint1 = arcCenter;
                Vector3 endPoint2 = arcCenter;

                if (arcForwardVector == Vector3.forward)
                {
                    endPoint1 += new Vector3(radius * Mathf.Cos(rad1), 0, radius * Mathf.Sin(rad1));
                    endPoint2 += new Vector3(radius * Mathf.Cos(rad2), 0, radius * Mathf.Sin(rad2));
                }
                else if (arcForwardVector == Vector3.up)
                {
                    endPoint1 += new Vector3(radius * Mathf.Cos(rad1), radius * Mathf.Sin(rad1), 0);
                    endPoint2 += new Vector3(radius * Mathf.Cos(rad2), radius * Mathf.Sin(rad2), 0);
                }

                Handles.DrawLine(arcCenter, endPoint1);
                Handles.DrawLine(arcCenter, endPoint2);
            }

            //Draws both outeredges
            Handles.DrawWireArc(arcCenter, arcUpVector, arcForwardVector, angle * 0.5f, radius);
            Handles.DrawWireArc(arcCenter, arcUpVector, arcForwardVector, -angle * 0.5f, radius);

            //Draws the filler
            Handles.color = new Color(color.r, color.g, color.b, alpha);
            Handles.DrawSolidArc(arcCenter, arcUpVector, arcForwardVector, angle * 0.5f, radius);
            Handles.DrawSolidArc(arcCenter, arcUpVector, arcForwardVector, -angle * 0.5f, radius);
        }
        else if (angle >= 360.01f)
        {
            DrawInfoText(arcCenter + arcForwardVector * radius, $"The angle is too big ({angle}?? / 360??)");
        }
        else if (angle <= 0f)
        {
            DrawInfoText(arcCenter + arcForwardVector * radius, $"The angle is too small ({angle}?? / 360??)");
        }
    }
    
    //Draw Simple Line
    static void Line(Vector3 startPoint, Vector3 endPoint, Color color, string pointName, bool showText)
    {
        Gizmos.color = color;
        Vector3 middlePoint = (((startPoint - endPoint) / 2) + endPoint);

        if (pointName != "" && showText)
        {
            //Start Point
            DrawInfoText(startPoint + new Vector3(0, 0.8f, 0), $"{pointName} 1\n{startPoint.ToString("F1")}");

            //End Point
            DrawInfoText(endPoint + new Vector3(0, 0.8f, 0), $"{pointName} 2\n{endPoint.ToString("F1")}");

            //Middle point
            DrawInfoText(middlePoint + new Vector3(0, 0.8f, 0),
                $"(Middle Point)\n{middlePoint.ToString("F1")}");
        }

        Gizmos.DrawLine(startPoint, endPoint);
        GetHoloGizmosMesh(GetDiamondMesh(), startPoint, Quaternion.identity, 0.3f * Vector3.one, color, 0.5f);
        GetHoloGizmosMesh(GetDiamondMesh(), middlePoint, Quaternion.identity, 0.3f * Vector3.one, color, 0.5f);
        GetHoloGizmosMesh(GetDiamondMesh(), endPoint, Quaternion.identity, 0.3f * Vector3.one, color, 0.5f);
    }

    //Draw Multi Line
    static void MultiLine(Vector3[] points, Color color, bool isLooping, string pointName, string[] transformNames, bool showText)
    {
        Gizmos.color = color;
        for (int i = 0; i < points.Length; i++)
        {
            GetHoloGizmosMesh(GetDiamondMesh(), points[i], Quaternion.identity, 0.3f * Vector3.one, color, 0.5f);

            if (pointName != "" && showText)
            {
                DrawInfoText(points[i] + new Vector3(0, 0.8f, 0),
                    $"{pointName} {i + 1} ({transformNames[i]})\n{points[i]}");
            }

            if (i + 1 < points.Length)
            {
                Gizmos.DrawLine(points[i + 1], points[i]);
            }
        }

        if (isLooping)
        {
            Gizmos.DrawLine(points[0], points[points.Length - 1]);
        }
    }

    //Draw Cylinder
    static void Cylinder(Vector3 position, Vector3 size, Color color, float alpha, bool showText)
    {
        Mesh mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");

        if (showText)
        {
        DrawInfoText(position + new Vector3(0, Mathf.Abs(0.5f + size.y), 0),
            $"Pos: {position.ToString("F1")}" +
            $"\nSize: {size.ToString("F1")}");
        }

        Gizmos.color = new Color(color.r, color.g, color.b, alpha);
        Gizmos.DrawMesh(mesh, 0, position, Quaternion.identity, size);
        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        Gizmos.DrawWireMesh(mesh, 0, position, Quaternion.identity, size);
    }

    //Draw Diamond
    static void Diamond(Vector3 position, Quaternion rotation, Vector3 size, Color color, float alpha, bool showText)
    {
        if (showText)
        {
        DrawInfoText(position + new Vector3(0, Mathf.Abs(size.y), 0),
            $"Pos: {position.ToString()}" +
            $"\nSize: {size.ToString()}");
        }

        GetHoloGizmosMesh(GetDiamondMesh(), position, Quaternion.identity, size, color, alpha);
    }

    //DrawPyramid
    static void HexPyramid(Mesh solidMesh, Mesh wireMesh, Vector3 position, Quaternion rotation, Vector3 size,
        Color color, float alpha, bool showText)
    {
        if (showText)
        {
        DrawInfoText(position + new Vector3(0, Mathf.Abs(size.y), 0),
            $"Pos: {position.ToString()}" +
            $"\nSize: {size.ToString()}");
        }

        Gizmos.color = new Color(color.r, color.g, color.b, alpha);
        Gizmos.DrawMesh(solidMesh, -1, position, rotation, size);

        Gizmos.color = new Color(color.r, color.g, color.b, 1f);
        Gizmos.DrawWireMesh(wireMesh, -1, position, rotation, size);
    }

    #endregion

    //Callable voids

    #region Callable voids

    //Default

    #region Cube

    /// <summary>
    /// Draws a holographic cube that follows the transform's position and rotation with size, color and alpha (alpha 0.0f - 1.0f)
    /// </summary>
    /// <param name="objectTransform">Takes the position of the transform and draws the cube there</param>
    /// <param name="size">The size of the cube</param>
    /// <param name="color">The color of the gizmo</param>
    /// <param name="alpha">The level of transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloCube(Transform objectTransform, Vector3 size, Color color, float alpha, bool showText)
    {
        Cube(objectTransform.position, size, color, alpha, showText);
    }

    /// <summary>
    /// Draws a holographic cube that follows the transform's position and rotation with size, and semi-transparent color
    /// </summary>
    /// <param name="objectTransform">Takes the position of the transform and draws the cube there</param>
    /// <param name="size">The size of the cube</param>
    /// <param name="color">The color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloCube(Transform objectTransform, Vector3 size, Color color, bool showText)
    {
        Cube(objectTransform.position, size, color, 0.3f, showText);
    }

    /// <summary>
    ///Draws a holographic cube that follows a position with size, and semi-transparent color
    /// </summary>
    /// <param name="position">Center position of the cube</param>
    /// <param name="size">The size of the cube</param>
    /// <param name="color">The color of the gizmo</param>
    /// <param name="alpha">The level of transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloCube(Vector3 position, Vector3 size, Color color, float alpha, bool showText)
    {
        Cube(position, size, color, alpha, showText);
    }

    /// <summary>
    /// Draws a holographic cube that follows a position with size, and semi-transparent color
    /// </summary>
    /// <param name="position">Center position of the cube</param>
    /// <param name="size">The size of the cube</param>
    /// <param name="color">The color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloCube(Vector3 position, Vector3 size, Color color, bool showText)
    {
        Cube(position, size, color, 0.3f, showText);
    }

    #endregion

    #region Sphere

    /// <summary>
    /// Draws a holographic sphere at position with size and semi-transparent color
    /// </summary>
    /// <param name="position">Position of the gizmo</param>
    /// <param name="radius">The radius of the sphere (from the middle and out)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloSphere(Vector3 position, float radius, Color color, bool showText)
    {
        Sphere(position, radius, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a holographic sphere at position with size, color and alpha (alpha 0.0f - 1.0f)
    /// </summary>
    /// <param name="position">Position of the gizmo</param>
    /// <param name="radius">The radius of the sphere (from the middle and out)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">The level of transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloSphere(Vector3 position, float radius, Color color, float alpha, bool showText)
    {
        Sphere(position, radius, color, alpha, showText);
    }

    /// <summary>
    /// Draws a holographic sphere at position with size, color and alpha (alpha 0.0f - 1.0f)
    /// </summary>
    /// <param name="objectTransform">Position of the gizmo</param>
    /// <param name="radius">The radius of the sphere (from the middle and out)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloSphere(Transform objectTransform, float radius, Color color, bool showText)
    {
        Sphere(objectTransform.position, radius, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a holographic sphere at position with size, color and alpha (alpha 0.0f - 1.0f)
    /// </summary>
    /// <param name="objectTransform">Position of the gizmo</param>
    /// <param name="radius">The radius of the sphere (from the middle and out)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">The level of transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloSphere(Transform objectTransform, float radius, Color color, float alpha, bool showText)
    {
        Sphere(objectTransform.position, radius, color, alpha, showText);
    }

    #endregion

    #region Mesh

    /// <summary>
    /// Draws a holographic mesh at a custom position, rotation, size and semi-transparent color
    /// </summary>
    /// <param name="mesh">What type of mesh should be drawn</param>
    /// <param name="objectPosition">Position of the gizmo</param>
    /// <param name="objectRotation">Rotation of the gizmo</param>
    /// <param name="objectScale">Scale of the gizmo</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloMesh(Mesh mesh,
        Vector3 objectPosition, Quaternion objectRotation, Vector3 objectScale, Color color, bool showText)
    {
        Mesh(mesh, objectPosition, objectRotation, objectScale, color, 0.5f, showText);
    }

    /// <summary>
    /// Draws a holographic mesh at a custom position, rotation, color and alpha (alpha 0.0f - 1.0f)
    /// </summary>
    /// <param name="mesh">What type of mesh should be drawn</param>
    /// <param name="objectPosition">Position of the gizmo</param>
    /// <param name="objectRotation">Rotation of the gizmo</param>
    /// <param name="objectScale">Scale of the gizmo</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloMesh(Mesh mesh,
        Vector3 objectPosition, Quaternion objectRotation, Vector3 objectScale,
        Color color, float alpha, bool showText)
    {
        Mesh(mesh, objectPosition, objectRotation, objectScale, color, alpha, showText);
    }

    /// <summary>
    /// Draws a holographic mesh at a set transform and semi-transparent color
    /// </summary>
    /// <param name="mesh">What type of mesh should be drawn</param>
    /// <param name="objectTransform">Object transform (position, rotation, scale)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloMesh(Mesh mesh, Transform objectTransform, Color color, bool showText)
    {
        Mesh(mesh, objectTransform.position, objectTransform.rotation, objectTransform.localScale,
            color, 0.5f, showText);
    }

    /// <summary>
    /// Draws a holographic mesh at a set transform and semi-transparent color
    /// </summary>
    /// <param name="mesh">What type of mesh should be drawn</param>
    /// <param name="objectTransform">Object transform (position, rotation, scale)</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHoloMesh(Mesh mesh, Transform objectTransform, Color color, float alpha, bool showText)
    {
        Mesh(mesh, objectTransform.position, objectTransform.rotation, objectTransform.localScale,
            color, alpha, showText);
    }

    #endregion

    #region Arc

    /// <summary>
    /// Draws an arc from the a given point in an angle
    /// </summary>
    /// <param name="arcCenter">Center of the arc</param>
    /// <param name="arcUpVector">The up/face vector of the arc</param>
    /// <param name="startPoint">The center of the arc</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The length of from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawArc(Vector3 arcCenter, Vector3 arcUpVector, Vector3 startPoint, float angle, float radius,
        Color color, bool showText)
    {
        Arc(arcCenter, arcUpVector, startPoint, angle, radius, color, 0.2f, showText);
    }

    /// <summary>
    /// Draws an arc from the a given point in an angle
    /// </summary>
    /// <param name="arcCenter">Center of the arc</param>
    /// <param name="arcUpVector">The up/face vector of the arc</param>
    /// <param name="startPoint">The center of the arc</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The length of from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawArc(Vector3 arcCenter, Vector3 arcUpVector, Vector3 startPoint, float angle, float radius,
        Color color, float alpha, bool showText)
    {
        Arc(arcCenter, arcUpVector, startPoint, angle, radius, color, alpha, showText);
    }

    /// <summary>
    /// Draws an arc from the a given point in an angle
    /// </summary>
    /// <param name="objectTransform">Center of the arc</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The length of from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawArc(Transform objectTransform, float angle, float radius, Color color, bool showText)
    {
        Arc(objectTransform.position, objectTransform.up, objectTransform.forward,
            angle, radius, color, 0.2f, showText);
    }

    /// <summary>
    /// Draws an arc from the a given point in an angle
    /// </summary>
    /// <param name="objectTransform">Center of the arc</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The length of from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawArc(Transform objectTransform, float angle, float radius,
        Color color, float alpha, bool showText)
    {
        Arc(objectTransform.position, objectTransform.up, objectTransform.forward,
            angle, radius, color, alpha, showText);
    }

    #endregion

    #region ViewArc

    /// <summary>
    /// Draw an arc that displays a view angle from a set center 
    /// </summary>
    /// <param name="arcCenter">Center of the arc</param>
    /// <param name="arcForwardVector">The forward axis of the angle</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The size from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawViewArc(Vector3 arcCenter, Vector3 arcForwardVector, float angle,
        float radius, Color color, bool showText)
    {
        ViewArc(arcCenter, arcForwardVector, angle, radius, color, 0.2f, showText);
    }

    /// <summary>
    /// Draw an arc that displays a view angle from a set center 
    /// </summary>
    /// <param name="arcCenter">Center of the arc</param>
    /// <param name="arcForwardVector">The forward axis of the angle</param>
    /// <param name="angle">The angle of the arc</param>
    /// <param name="radius">The size from the center</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawViewArc(Vector3 arcCenter, Vector3 arcForwardVector, float angle,
        float radius, Color color, float alpha, bool showText)
    {
        ViewArc(arcCenter, arcForwardVector, angle, radius, color, alpha, showText);
    }

    #endregion

    #region Lines

    #region Transforms

    /// <summary>
    /// Draws a straight line between two points and marks a middlepoint
    /// </summary>
    /// <param name="startPoint">The first point</param>
    /// <param name="endPoint">The last point</param>
    /// <param name="color">Custom color for the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Transform startPoint, Transform endPoint, Color color, bool showText)
    {
        Line(startPoint.position, endPoint.position, color, "", showText);
    }

    /// <summary>
    /// Draws a straight line between two points and marks a middlepoint
    /// </summary>
    /// <param name="startPoint">The first point</param>
    /// <param name="endPoint">The last point</param>
    /// <param name="color">Custom color for the gizmo</param>
    /// <param name="pointName"> Gives the point a unique name</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Transform startPoint, Transform endPoint, Color color,
        string pointName, bool showText)
    {
        Line(startPoint.position, endPoint.position, color, pointName, showText);
    }

    /// <summary>
    ///Draws a line between all points of an array of transfroms
    /// </summary>
    /// <param name="points">A array of points to draw line between</param>
    /// <param name="color">Custom colors to display the gizmo</param>
    /// <param name="loop">Should the lines loop</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Transform[] points, Color color, bool loop, bool showText)
    {
        Vector3[] pointsPos = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i].position;
        }

        MultiLine(pointsPos, color, loop, "", null, showText);
    }

    /// <summary>
    ///Draws a line between all points of an array of transfroms
    /// </summary>
    /// <param name="points">A array of points to draw line between</param>
    /// <param name="color">Custom colors to display the gizmo</param>
    /// <param name="loop">Should the lines loop</param>
    /// <param name="pointName">Give the points a unique name</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Transform[] points, Color color, bool loop, string pointName, bool showText)
    {
        Vector3[] pointsPos = new Vector3[points.Length];
        string[] pointNames = new string[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i].position;
            pointNames[i] = points[i].name;
        }

        MultiLine(pointsPos, color, loop, pointName, pointNames, showText);
    }

    #endregion

    #region Vector3

    /// <summary>
    /// Draws a straight line between two points and marks a middlepoint
    /// </summary>
    /// <param name="startPoint">The first point</param>
    /// <param name="endPoint">The last point</param>
    /// <param name="color">Custom color for the gizmo</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Vector3 startPoint, Vector3 endPoint, Color color, bool showText)
    {
        Line(startPoint, endPoint, color, "", showText);
    }

    /// <summary>
    /// Draws a straight line between two points and marks a middlepoint
    /// </summary>
    /// <param name="startPoint">The first point</param>
    /// <param name="endPoint">The last point</param>
    /// <param name="color">Custom color for the gizmo</param>
    /// <param name="pointName"> Gives the point a unique name</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Vector3 startPoint, Vector3 endPoint, Color color,
        string pointName, bool showText)
    {
        Line(startPoint, endPoint, color, pointName, showText);
    }

    /// <summary>
    ///Draws a line between all points of an array of transfroms
    /// </summary>
    /// <param name="points">A array of points to draw line between</param>
    /// <param name="color">Custom colors to display the gizmo</param>
    /// <param name="loop">Should the lines loop</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Vector3[] points, Color color, bool loop, bool showText)
    {
        Vector3[] pointsPos = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i];
        }

        MultiLine(pointsPos, color, loop, "", null, showText);
    }

    /// <summary>
    ///Draws a line between all points of an array of transfroms
    /// </summary>
    /// <param name="points">A array of points to draw line between</param>
    /// <param name="color">Custom colors to display the gizmo</param>
    /// <param name="loop">Should the lines loop</param>
    /// <param name="pointName"> Gives the point a unique name</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawLine(Vector3[] points, Color color, bool loop, string pointName, bool showText)
    {
        Vector3[] pointsPos = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsPos[i] = points[i];
        }

        MultiLine(pointsPos, color, loop, pointName, null, showText);
    }

    #endregion

    #endregion

    #region Cylinder

    /// <summary>
    /// Draws a cylinder
    /// </summary>
    /// <param name="position">Center position of the cylinder</param>
    /// <param name="size">Size of the cylinder</param>
    /// <param name="color">Color of the cylinder</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawCylinder(Vector3 position, Vector3 size, Color color, bool showText)
    {
        Cylinder(position, size, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a cylinder
    /// </summary>
    /// <param name="position">Center position of the cylinder</param>
    /// <param name="size">Size of the cylinder</param>
    /// <param name="color">Color of the cylinder</param>
    /// <param name="alpha">Transparency of the diamond's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawCylinder(Vector3 position, Vector3 size, Color color, float alpha, bool showText)
    {
        Cylinder(position, size, color, alpha, showText);
    }

    /// <summary>
    /// Draws a cylinder
    /// </summary>
    /// <param name="objectTransform">Center position and scale of the cylinder</param>
    /// <param name="color">Color of the cylinder</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawCylinder(Transform objectTransform, Color color, bool showText)
    {
        Cylinder(objectTransform.position, objectTransform.localScale, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a cylinder
    /// </summary>
    /// <param name="objectTransform">Center position and scale of the cylinder</param>
    /// <param name="color">Color of the cylinder</param>
    /// <param name="alpha">Transparency of the diamond's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawCylinder(Transform objectTransform, Color color, float alpha, bool showText)
    {
        Cylinder(objectTransform.position, objectTransform.localScale, color, alpha, showText);
    }

    #endregion

    //Custom made shapes

    #region Diamond

    /// <summary>
    /// Draws a octahedron (8 sided diamond)
    /// </summary>
    /// <param name="position">Position of the diamond</param>
    /// <param name="rotation">Rotation of the diamond</param>
    /// <param name="size">Size of the diamond</param>
    /// <param name="color">Color of the diamond</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawDiamond(Vector3 position, Quaternion rotation, float size, Color color, bool showText)
    {
        Diamond(position, rotation, size * Vector3.one, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a octahedron (8 sided diamond)
    /// </summary>
    /// <param name="position">Position of the diamond</param>
    /// <param name="rotation">Rotation of the diamond</param>
    /// <param name="size">Size of the diamond</param>
    /// <param name="color">Color of the diamond</param>
    /// <param name="alpha">Transparency of the diamond's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawDiamond(Vector3 position, Quaternion rotation, float size, Color color, float alpha, bool showText)
    {
        Diamond(position, rotation, size * Vector3.one, color, alpha, showText);
    }

    /// <summary>
    /// Draws a octahedron (8 sided diamond)
    /// </summary>
    /// <param name="objectTransform">Transform of the diamond</param>
    /// <param name="color">Color of the diamond</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawDiamond(Transform objectTransform, Color color, bool showText)
    {
        Vector3 position = objectTransform.position;
        Quaternion rotation = objectTransform.rotation;
        Vector3 size = objectTransform.localScale;

        Diamond(position, rotation, size, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a octahedron (8 sided diamond) 
    /// </summary>
    /// <param name="objectTransform">Transform of the diamond</param>
    /// <param name="color">Color of the diamond</param>
    /// <param name="alpha">Transparency of the diamond's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawDiamond(Transform objectTransform, Color color, float alpha, bool showText)
    {
        Vector3 position = objectTransform.position;
        Quaternion rotation = objectTransform.rotation;
        Vector3 size = objectTransform.localScale;

        Diamond(position, rotation, size, color, alpha, showText);
    }

    #endregion

    #region HexaPyramid

    /// <summary>
    /// Draws a hexagonal pyramid (Low poly pyramid)
    /// </summary>
    /// <param name="position">Position of the hexagonal pyramid</param>
    /// <param name="rotation">Rotation of the hexagonal pyramid</param>
    /// <param name="size">Size of the hexagonal pyramid</param>
    /// <param name="color">Color of the hexagonal pyramid</param>
    /// <param name="alpha">Transparency of the hexagonal pyramid's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHexagonalPyramid(Vector3 position, Quaternion rotation, Vector3 size,
        Color color, float alpha, bool showText)
    {
        HexPyramid(GetHexaPyramid().Item1, GetHexaPyramid().Item2,
            position, rotation, size, color, alpha, showText);
    }

    /// <summary>
    /// Draws a hexagonal pyramid (Low poly pyramid)
    /// </summary>
    /// <param name="position">Position of the hexagonal pyramid</param>
    /// <param name="rotation">Rotation of the hexagonal pyramid</param>
    /// <param name="size">Size of the hexagonal pyramid</param>
    /// <param name="color">Color of the hexagonal pyramid</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHexagonalPyramid(Vector3 position, Quaternion rotation, Vector3 size, Color color, bool showText)
    {
        HexPyramid(GetHexaPyramid().Item1, GetHexaPyramid().Item2,
            position, rotation, size, color, 0.3f, showText);
    }

    /// <summary>
    /// Draws a hexagonal pyramid (Low poly pyramid)
    /// </summary>
    /// <param name="objectTransform">Transform of the hexagonal pyramid</param>
    /// <param name="color">Color of the hexagonal pyramid</param>
    /// <param name="alpha">Transparency of the hexagonal pyramid's volume</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHexagonalPyramid(Transform objectTransform, Color color, float alpha, bool showText)
    {
        HexPyramid(GetHexaPyramid().Item1, GetHexaPyramid().Item2,
            objectTransform.position, objectTransform.rotation, objectTransform.localScale,
            color, alpha, showText);
    }

    /// <summary>
    /// Draws a hexagonal pyramid (Low poly pyramid)
    /// </summary>
    /// <param name="objectTransform">Transform of the hexagonal pyramid</param>
    /// <param name="color">Color of the hexagonal pyramid</param>
    /// <param name="showText">Display Position, Rotation, Scale</param>
    public static void DrawHexagonalPyramid(Transform objectTransform, Color color, bool showText)
    {
        HexPyramid(GetHexaPyramid().Item1, GetHexaPyramid().Item2,
            objectTransform.position, objectTransform.rotation, objectTransform.localScale,
            color, 0.3f, showText);
    }

    #endregion

    #endregion

    //Development Stuff

    #region [Development Tools]

    /// <summary>
    /// Draws a box that displays information
    /// </summary>
    /// <param name="appearPoint">The position the box should appear</param>
    /// <param name="infoText">The content of the box</param>
    static void DrawInfoText(Vector3 appearPoint, string infoText)
    {
        Handles.Label(appearPoint, infoText, new GUIStyle("Box"));
    }

    /// <summary>
    /// Gets the mesh data for a octahedron (8 sided diamond) shaped mesh
    /// </summary>
    /// <returns>Octahedron mesh</returns>
    static Mesh GetDiamondMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[24];

        // Top/Bot
        vertices[0] = new Vector3(0, 0.5f, 0); //Top
        vertices[1] = new Vector3(0, -0.5f, 0); //Bot

        //Corners
        vertices[2] = new Vector3(-0.25f, 0, -0.25f); //Down-Left (0,0)
        vertices[3] = new Vector3(0.25f, 0, -0.25f); //Down-Right (1,0)
        vertices[4] = new Vector3(-0.25f, 0, 0.25f); //Up-Left (0,1)
        vertices[5] = new Vector3(0.25f, 0, 0.25f); //Up-Right (1,1)

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);
        uv[4] = new Vector2(0, 1);
        uv[5] = new Vector2(0, 1);

        #region Triangels

        triangles[0] = 3;
        triangles[1] = 1;
        triangles[2] = 2; //X-Posiive Top
        triangles[3] = 2;
        triangles[4] = 0;
        triangles[5] = 3; //X-Posiive Bot
        triangles[6] = 5;
        triangles[7] = 1;
        triangles[8] = 3; //X-Posiive Top
        triangles[9] = 3;
        triangles[10] = 0;
        triangles[11] = 5; //X-Negative Bot

        triangles[12] = 4;
        triangles[13] = 1;
        triangles[14] = 5; //Z-Posiive Top
        triangles[15] = 5;
        triangles[16] = 0;
        triangles[17] = 4; //Z-Posiive Bot
        triangles[18] = 4;
        triangles[19] = 0;
        triangles[20] = 2; //Z-Negative Top
        triangles[21] = 2;
        triangles[22] = 1;
        triangles[23] = 4; //Z-Negative Bot

        #endregion

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Gets the mesh data for a hexagonal pyramid shaped mesh
    /// </summary>
    /// <returns>Hexagonal pyramid mesh</returns>
    static (Mesh, Mesh) GetHexaPyramid()
    {
        Mesh wiredMesh = new Mesh();
        Mesh solidMesh = new Mesh();

        Vector3[] vertices = new Vector3[9];
        int[] solidTriangles = new int[36];
        int[] wiredTriangles = new int[18];

        vertices[0] = new Vector3(0, 0.5f, 0); //Tip

        vertices[2] = new Vector3(0.25f, -0.5f, 0.45f); //2
        vertices[1] = new Vector3(-0.25f, -0.5f, 0.45f); //1
        vertices[6] = new Vector3(-0.5f, -0.5f, 0f); //6
        vertices[3] = new Vector3(0.5f, -0.5f, 0f); //3
        vertices[5] = new Vector3(-0.25f, -0.5f, -0.45f); //5
        vertices[4] = new Vector3(0.25f, -0.5f, -0.45f); //4

        vertices[7] = new Vector3(0, -0.5f, 0); //Bot 

        #region Triangles

        wiredTriangles[0] = 2;
        wiredTriangles[1] = 0;
        wiredTriangles[2] = 1; //Side Face 1
        wiredTriangles[3] = 3;
        wiredTriangles[4] = 0;
        wiredTriangles[5] = 2; // Side Face 2
        wiredTriangles[6] = 4;
        wiredTriangles[7] = 0;
        wiredTriangles[8] = 3; //Side Face 4
        wiredTriangles[9] = 5;
        wiredTriangles[10] = 0;
        wiredTriangles[11] = 4; //Side Face 5
        wiredTriangles[12] = 6;
        wiredTriangles[13] = 0;
        wiredTriangles[14] = 5; //Side Face 6
        wiredTriangles[15] = 1;
        wiredTriangles[16] = 0;
        wiredTriangles[17] = 6; //Side Face 7

        solidTriangles[0] = 2;
        solidTriangles[1] = 0;
        solidTriangles[2] = 1; //Side Face 1
        solidTriangles[3] = 3;
        solidTriangles[4] = 0;
        solidTriangles[5] = 2; // Side Face 2
        solidTriangles[6] = 4;
        solidTriangles[7] = 0;
        solidTriangles[8] = 3; //Side Face 4
        solidTriangles[9] = 5;
        solidTriangles[10] = 0;
        solidTriangles[11] = 4; //Side Face 5
        solidTriangles[12] = 6;
        solidTriangles[13] = 0;
        solidTriangles[14] = 5; //Side Face 6
        solidTriangles[15] = 1;
        solidTriangles[16] = 0;
        solidTriangles[17] = 6; //Side Face 7

        solidTriangles[18] = 1;
        solidTriangles[19] = 7;
        solidTriangles[20] = 2; //Bottom Triangle 1
        solidTriangles[21] = 2;
        solidTriangles[22] = 7;
        solidTriangles[23] = 3; //Bottom Triangle 2
        solidTriangles[24] = 3;
        solidTriangles[25] = 7;
        solidTriangles[26] = 4; //Bottom Triangle 4
        solidTriangles[27] = 4;
        solidTriangles[28] = 7;
        solidTriangles[29] = 5; //Bottom Triangle 5
        solidTriangles[30] = 5;
        solidTriangles[31] = 7;
        solidTriangles[32] = 6; //Bottom Triangle
        solidTriangles[33] = 6;
        solidTriangles[34] = 7;
        solidTriangles[35] = 1; //Bottom Triangle 7

        #endregion

        solidMesh.vertices = vertices;
        solidMesh.triangles = solidTriangles;
        solidMesh.RecalculateNormals();

        wiredMesh.vertices = vertices;
        wiredMesh.triangles = wiredTriangles;
        wiredMesh.normals = vertices;

        return (solidMesh, wiredMesh);
    }

    /// <summary>
    /// Draw the wireframe and volume of a mesh
    /// </summary>
    /// <param name="mesh">Mesh to be drawn</param>
    /// <param name="position">Position of the gizmo</param>
    /// <param name="rotation">Rotation of the gizmo</param>
    /// <param name="size">Size of the gizmo</param>
    /// <param name="color">Color of the gizmo</param>
    /// <param name="alpha">Transparency of the gizmo's volume</param>
    /// <returns>Gizmo of a mesh, both volume and wireframe</returns>
    static Gizmos GetHoloGizmosMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 size, Color color,
        float alpha)
    {
        Gizmos meshGizmo = new Gizmos();
        Gizmos.color = new Color(color.r, color.g, color.b, alpha);
        Gizmos.DrawMesh(mesh, position, rotation, size);
        Gizmos.color = color;
        Gizmos.DrawWireMesh(mesh, position, rotation, size);

        return meshGizmo;
    }

    #endregion
}
