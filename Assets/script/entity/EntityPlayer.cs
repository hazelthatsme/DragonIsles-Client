using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{
    Camera cam;
    Vector3 rotation;
    Vector3 velocity;
    Vector3 force;
    Rigidbody rbody;
    public float maxVelocityChange = 10.0f;
    public float sensitivity = 50.0f;
    World world;
    bool jump;

    void Start()
    {
        cam = Camera.main;
        rbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        world = GameObject.Find("World").GetComponent<World>();
        world.OnTick += UpdatePresence;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        force = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * attributes.MOVEMENT_SPEED;
        rotation = new Vector3(Mathf.Clamp(rotation.x - (Input.GetAxis("Turn Vertical") * sensitivity), -90, 90), rotation.y + (Input.GetAxis("Turn Horizontal") * sensitivity), 0);

        if (Input.GetButtonUp("Jump")) jump = true;
    }

    void UpdatePresence(object sender, TickEventArgs e)
    {
        GetComponent<DiscordController>().presence.largeImageKey = "dragonisles";
        GetComponent<DiscordController>().presence.smallImageKey = "icon-spacing";
        GetComponent<DiscordController>().presence.details = "Among the Dragon Isles";
        GetComponent<DiscordController>().presence.startTimestamp = world.startTime;
        GetComponent<DiscordController>().UpdatePresence();
    }

    void FixedUpdate()
    {
        /*Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Debug.Log(targetVelocity);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= attributes.MOVEMENT_SPEED;
        targetVelocity *= 5;

        Vector3 velocity = rbody.velocity;
        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        if (jump)
        {
            velocityChange.y = attributes.JUMP_FORCE;
            jump = false;
        }
        else velocityChange.y = 0;
        rbody.AddForce(velocityChange, ForceMode.VelocityChange);*/
        if (jump)
        {
            force.y = attributes.JUMP_FORCE;
            jump = false;
        }
        else force.y = rbody.velocity.y;

        force = transform.TransformDirection(force);

        rbody.velocity = force;

        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
        cam.transform.localRotation = Quaternion.Euler(new Vector3(rotation.x, 0));
    }
}
