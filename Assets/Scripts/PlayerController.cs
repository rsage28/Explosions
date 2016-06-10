﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float speed = 6f;            // The speed that the player will move at.
    public Bullet bullet;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.
    bool canFire;

    void Awake() {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask("Floor");

        playerRigidbody = GetComponent<Rigidbody>();
        canFire = true;
    }

    void Update() {
        if (Input.GetButton("Fire1")) {
            GameObject gun = transform.FindChild("Gun").gameObject;
            if (gun != null) {
                Transform gunEnd = gun.transform.FindChild("Gun End");
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Create a RaycastHit variable to store information about what was hit by the ray.
                RaycastHit floorHit;

                // Perform the raycast and if it hits something on the floor layer...
                if (canFire && Physics.Raycast(camRay, out floorHit, camRayLength)) {
                    Bullet proj = (Bullet) Instantiate(bullet, gunEnd.position, Quaternion.identity);
                    Vector3 target = floorHit.point;
                    target.y = 1.25f;
                    proj.transform.LookAt(target);
                    proj.MyRigidBody.velocity = proj.transform.forward * 35 + (movement * 10);
                    canFire = false;
                    StartCoroutine(waitBetweenBullets());
                }
            }
        }
    }

    IEnumerator waitBetweenBullets() {
        yield return new WaitForSeconds(.5f);
        canFire = true;
    }

    void FixedUpdate() {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Move the player around the scene.
        Move(h, v);

        // Turn the player to face the mouse cursor.
        Turning();
    }

    void Move(float h, float v) {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning() {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            playerRigidbody.MoveRotation(newRotation);
        }
    }
}
