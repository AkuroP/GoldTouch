using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
public class GemsManager : MonoBehaviour
{
    private int _nextGem;

    [SerializeField]
    private GameObject _currentGem;
    private Rigidbody _currentGemRb;
    private Vector2 _tapPos;
    [SerializeField]
    private Transform _spawnPoint;
    public float _cooldown = 1f;
    private float _cd;

    [Foldout("Wall"), SerializeField]
    private Transform _wallLeft;
    [Foldout("Wall"), SerializeField]
    private Transform _wallRight;

    [SerializeField]
    private Sprite[] _gemsSprites;
    [SerializeField]
    private Image _nextImage;
    // Start is called before the first frame update
    void Start()
    {
        _nextGem = Random.Range(0, 4);
        _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);
        _currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();
        _currentGemRb.isKinematic = true;
        _cd = _cooldown;

        _nextGem = Random.Range(0, 4);
        _nextImage.sprite = _gemsSprites[_nextGem];
        //_affichageGems[_nextGem].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Win) return;
        if (GameManager.instance.CanPlay) return;
        if (_cd > 0) _cd -= Time.deltaTime;
        else
        {
            NextGem();
            GameManager.instance.CanPlay = true;
        }


    }

    public void NextGem()
    {
        if (GameManager.instance.Win) return;

        //_affichageGems[_nextGem].SetActive(false);
        _cd = _cooldown;
        if(_currentGem == null) _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);
        _currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();
        _currentGemRb.isKinematic = true;
        _nextGem = Random.Range(0, 4);
        _nextImage.sprite = _gemsSprites[_nextGem];

        //_affichageGems[_nextGem].SetActive(true);
        //Debug.Log("Next gem is : " + _nextGem);
    }


    public void OnTouchDrag(InputAction.CallbackContext ctx)
    {
        if (GameManager.instance.Win) return;

        if (!GameManager.instance.CanPlay) return;
        if (_currentGem == null) return;
        
        _tapPos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        if(ctx.started)
        {
            if (GameManager.instance.CanPlay && _cd <= 0 && !GameManager.instance._inCombo) _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);
            else return;
        }
        if (ctx.performed)
        {
            _tapPos = Camera.main.ScreenToWorldPoint(new Vector3(ctx.ReadValue<Vector2>().x, 0f, 10f));
            _tapPos.y = _spawnPoint.position.y;

            Debug.Log(_tapPos);


            /*if (_tapPos.x > _wallLeft.position.x && _tapPos.x < _wallRight.position.x)*/ _currentGem.transform.position = new Vector2(_tapPos.x, _tapPos.y);
            if (_tapPos.x <= _wallLeft.position.x) _currentGem.transform.position = _wallLeft.position;
            else if (_tapPos.x >= _wallRight.position.x) _currentGem.transform.position = _wallRight.position;
        }
        if (ctx.canceled)
        {
            if (_currentGem == null) return;
            //Debug.Log("DROP");
            if(_currentGemRb == null)_currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();
            _currentGemRb.isKinematic = false;
            _currentGemRb.AddTorque(new Vector3(1, 0, 1), ForceMode.Impulse);
            GameManager.instance.nbPlay += 1;
            _currentGem = null;
            if(GameManager.instance.CanPlay) GameManager.instance.CanPlay = false;
        }
        
    }

}
