using UnityEngine;
using System.Collections;

public class CrosshairsController : MonoBehaviour
{
    public Vector2i target; // coordinates of unit being attacked
    public Vector3 displacement; // vector from start to target
    private float timer; // keeps track of how much time has passed, after 0.8 seconds snap to destination and blink
    private float prevTimer; // keeping track of intervals
    private bool moving = true;

    public AudioClip sfx;

    // Use this for initialization
    void Start()
    {
        // get displacement
        displacement = GLOBAL.gridToWorld(target) - transform.position;

        timer = 0;

        // disable UI until lockon complete
        UIManager.Instance.setUnitUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        prevTimer = timer;
        timer += Time.deltaTime;

        // handles crosshair movement
        if (moving)
        {
            if (timer < 0.8f)
            {
                transform.position += displacement * (Time.deltaTime / 0.8f);
            }

            else
            {
                transform.position = GLOBAL.gridToWorld(target);
                moving = false;
                timer = 0;
                BlinkOff();
            }
        }
        // handles crosshair blinking
        else
        { // invoke's timing is really bizarre so hardcode it like a scrub
            if (timer >= 0.125f && prevTimer < 0.125f)
            {
                Camera.main.GetComponent<AudioSource>().PlayOneShot(sfx);
                BlinkOn();
            }
            else if (timer >= 0.2f && prevTimer < 0.2f)
            {
                BlinkOff();
            }
            else if (timer >= 0.325f && prevTimer < 0.325f)
            {
                BlinkOn();
            }
            else if (timer >= 0.4f && prevTimer < 0.4f)
            {
                BlinkOff();
            }
            else if (timer >= 0.525f && prevTimer < 0.525f)
            {
                BlinkOn();
            }
            else if (timer >= 0.6f && prevTimer < 0.6f)
            {
                Finish();
            }
        }
    }

    void BlinkOff()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void BlinkOn()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    void Finish()
    { //send signal to display pre-combat information
        if (PlayerManager.Instance.getCurrentPlayerTurn() == PlayerManager.Instance.getCurrentPlayer().playerID)
        {
            CombatSequence.Instance.Begin();
        }
        // reactivate unit UI
        if (PlayerManager.Instance.getCurrentPlayer().playerID == 1 || GameDirector.Instance.isMultiPlayer())
        {
            UIManager.Instance.setUnitUI(true);
            UIManager.Instance.activateAttackButton();
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
//            stream.SendNext(transform.position);
            stream.SendNext(moving);
            stream.SendNext(timer);
            stream.SendNext(prevTimer);

            stream.SendNext(target.x);
            stream.SendNext(target.y);

        }
        else
        {
            // Network player, receive data
//            this.transform.position = (Vector3)stream.ReceiveNext();
            moving = (bool)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();
            prevTimer = (float)stream.ReceiveNext();


            target.x = (int)stream.ReceiveNext();
            target.y = (int)stream.ReceiveNext();

        }
    }
}
