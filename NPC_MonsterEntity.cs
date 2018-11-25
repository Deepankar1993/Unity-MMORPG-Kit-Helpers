using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using LiteNetLibManager;
using MultiplayerARPG;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LiteNetLibIdentity))]
[RequireComponent(typeof(LiteNetLibTransform))]
[RequireComponent(typeof(CharacterAnimationComponent))]
[RequireComponent(typeof(CharacterRecoveryComponent))]
[RequireComponent(typeof(CharacterSkillAndBuffComponent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(MonsterActivityComponent))]
[RequireComponent(typeof(MonsterCharacterEntity))]
[RequireComponent(typeof(LiteNetLibVisibleChecker))]
[ExecuteInEditMode]
public class NPC_MonsterEntity : MonoBehaviour
{
    [SerializeField] private GameObject Parent;
    [SerializeField] private GameObject DummyGO;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterModel characterModel;
    [SerializeField] private MonsterCharacterEntity monsterCharacterEntity;

    private GameObject go;
    
    //TODO :: Add Different Transform for Missile Damage Transform and Combat Text Transform 
    [Header("Damage Transfrom")] 
    [SerializeField]
    private GameObject _damageTransform;

    [SerializeField] private Vector3 _damageTransformPosition = new Vector3(0f, 1.2f, 0.25f);

    [Header("BuffBody Transfrom")] 
    [SerializeField]
    private GameObject _buffBody;

    [SerializeField] private Vector3 _buffBodyTransformPosition = new Vector3(0f, 1f, 0.25f);

    [Header("UIElementContainer")] 
    [SerializeField]
    private GameObject _uIElementContainer;

    [SerializeField] private Vector3 _uIElementContainerTransformPosition = new Vector3(0f, 0.15f, 0f);

    [Header("MiniMap Container")] 
    [SerializeField]
    private GameObject _miniMapContainer;

    [SerializeField] private Vector3 _miniMapContainerTransformPosition = new Vector3(0f, -0.15f, 0.25f);


    void Awake()
    {
        Debug.Log("Editor causes this Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Called in Editor");
        SetPosition();

        animator = gameObject.GetComponent<Animator>();
        characterModel = gameObject.GetComponent<CharacterModel>();
        AddGO();
    }

    private void SetPosition()
    {
        if (_damageTransformPosition == Vector3.zero || _buffBodyTransformPosition == Vector3.zero ||
            _uIElementContainerTransformPosition == Vector3.zero || _miniMapContainerTransformPosition == Vector3.zero)
        {
            _damageTransformPosition = new Vector3(0f, 1.2f, 0.25f);
            _buffBodyTransformPosition = new Vector3(0f, 1f, 0.25f);
            _uIElementContainerTransformPosition = new Vector3(0f, 0.15f, 0f);
            _miniMapContainerTransformPosition = new Vector3(0f, -0.15f, 0.25f);
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            SetupVaribles();
            UpdateAllData();
            if (Parent != null)
                AddGO();

            /*animator = gameObject.GetComponent<Animator>();
            characterModel = GetComponent<CharacterModel>();
            if ( characterModel.animatorController != animator)
            {
                characterModel.animatorController = animator.runtimeAnimatorController;
            }
            else
            {
                animator = gameObject.GetComponent<Animator>();
            }
            */


            Debug.Log("Editor causes this Update");
            animator = gameObject.GetComponent<Animator>();
        }
    }

    void SetupVaribles()
    {
        animator = animator != gameObject.GetComponent<Animator>() ? gameObject.GetComponent<Animator>() : animator;
        characterModel = characterModel != GetComponent<CharacterModel>()
            ? GetComponent<CharacterModel>()
            : characterModel;
        monsterCharacterEntity = monsterCharacterEntity != GetComponent<MonsterCharacterEntity>()
            ? GetComponent<MonsterCharacterEntity>()
            : monsterCharacterEntity;
    }

    void UpdateAllData()
    {
        characterModel.animatorController = characterModel.animatorController != animator
            ? animator.runtimeAnimatorController
            : characterModel.animatorController;
        //Damage Transform 
/*        monsterCharacterEntity.meleeDamageTransform = monsterCharacterEntity.meleeDamageTransform != _damageTransform
            ? _damageTransform.transform
            : monsterCharacterEntity.meleeDamageTransform;*/
    }

    GameObject MakeDummyGO()
    {
        if (DummyGO == null)
        {
            DummyGO = new GameObject();
        }

        return DummyGO;
    }

    void AddGO()
    {
        if (!Application.isEditor)
            return;
        SetPosition();
        if (Parent == null)
            return;
        
        if (CheckTranformStatus(_damageTransform))
        {
            /*GameObject go = new GameObject();
            go.name = "_DamageTransform";*/

            _damageTransform = CheckIfTransformAlreadyAvailable("_DamageTransform", _damageTransformPosition);
            monsterCharacterEntity.meleeDamageTransform = _damageTransform.transform;
            monsterCharacterEntity.combatTextTransform = _damageTransform.transform;
            monsterCharacterEntity.missileDamageTransform = _damageTransform.transform;
            Debug.Log("Added Damage Transform and Updated Monster Character Entity Script");
        }

        if (CheckTranformStatus(_buffBody))
        {
            /*GameObject go = new GameObject();
            go.name = "_BuffBody";*/

            _buffBody = CheckIfTransformAlreadyAvailable("_BuffBody", _buffBodyTransformPosition);
            //TODO :: Find where buff body is used 
            Debug.Log("Added  Buff Body Gameobject");
        }

        if (CheckTranformStatus(_uIElementContainer))
        {
            /*GameObject go = new GameObject();
            go.name = "_UIElementContainer";*/

            _uIElementContainer =
                CheckIfTransformAlreadyAvailable("_UIElementContainer", _uIElementContainerTransformPosition);
            monsterCharacterEntity.uiElementTransform = _uIElementContainer.transform;
            Debug.Log("Added uiElementTransform Transform and Updated  Monster Character Entity Script");
        }

        if (CheckTranformStatus(_miniMapContainer))
        {
            /*GameObject go = new GameObject();
            go.name = "_MiniMapContainer";*/

            _miniMapContainer =
                CheckIfTransformAlreadyAvailable("_MiniMapContainer", _miniMapContainerTransformPosition);
            monsterCharacterEntity.miniMapElementContainer = _miniMapContainer.transform;
            Debug.Log("Added miniMapContainer Transform and Updated  Monster Character Entity Script");
        }
    }

    bool CheckTranformStatus(GameObject go)
    {
        if (go == null || this.transform.Find(go.name) != null || this.transform.Find(go.name) != go.transform)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    GameObject CheckIfTransformAlreadyAvailable(string goname, Vector3 position)
    {

        
        if (this.transform.Find(goname) != null)
        {
            return this.transform.Find(goname).gameObject;
        }
        else
        {
            MakeDummyGO();
            GameObject nGo = Instantiate(DummyGO, position, Quaternion.identity);
            nGo.name = goname;
           nGo.transform.parent = Parent.transform;
            
            Debug.Log("nGo  name :: " + nGo.name);
            Debug.Log("Parent name :: " + gameObject.name);
            return nGo;
        }
    }
}