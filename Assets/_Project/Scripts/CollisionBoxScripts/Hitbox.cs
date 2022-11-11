﻿using Demonics.Enum;
using Demonics.Utility;
using System;
using UnityEngine;

public class Hitbox : DemonicsCollider
{
    public Action OnGroundCollision;
    public Action OnPlayerCollision;
    public DemonicsVector2 HitPoint { get; private set; }
    [SerializeField] private bool _hitGround;
    [SerializeField] private IHitboxResponder _hitboxResponder;
    protected Transform _sourceTransform;
    public bool HitConfirm { get; private set; }

    void Awake()
    {
        GizmoColor = Color.red;
        _sourceTransform = transform.root;
    }

    protected override void Start()
    {
        base.Start();
        if (_hitboxResponder == null)
        {
            _hitboxResponder = transform.root.GetComponent<IHitboxResponder>();
        }
    }

    protected override bool Colliding(DemonicsCollider a, DemonicsCollider b)
    {
        if (a._physics.Position.x > b._physics.Position.x)
        {
            HitPoint = new DemonicsVector2(a.Position.x - (a.Size.x / (DemonicsFloat)2), HitPoint.y);
        }
        else
        {
            HitPoint = new DemonicsVector2(a.Position.x + (a.Size.x / (DemonicsFloat)2), HitPoint.y);
        }
        if (a._physics.Position.y == b._physics.Position.y)
        {
            HitPoint = new DemonicsVector2(HitPoint.x, a.Position.y);
        }
        else if (a._physics.Position.y >= b._physics.Position.y)
        {
            HitPoint = new DemonicsVector2(HitPoint.x, a.Position.y - (a.Size.y / (DemonicsFloat)2));
        }
        else
        {
            HitPoint = new DemonicsVector2(HitPoint.x, a.Position.y + (a.Size.y / (DemonicsFloat)2));
        }
        return base.Colliding(a, b);
    }

    protected override void InitializeCollisionList()
    {
        DemonicsCollider[] demonicsCollidersArray = FindObjectsOfType<DemonicsCollider>();
        for (int i = 0; i < demonicsCollidersArray.Length; i++)
        {
            if (!demonicsCollidersArray[i].transform.IsChildOf(_sourceTransform))
            {
                if (demonicsCollidersArray[i].TryGetComponent(out Hurtbox hurtbox))
                {
                    _demonicsColliders.Add(demonicsCollidersArray[i]);
                }
            }
        }
        _demonicsColliders.Remove(this);
    }

    public void SetSourceTransform(Transform sourceTransform)
    {
        _sourceTransform = sourceTransform;
    }

    public void SetHitboxResponder(Transform hitboxResponder)
    {
        _hitboxResponder = hitboxResponder.GetComponent<IHitboxResponder>();
    }

    public void SetBox(Vector2 size, Vector2 offset)
    {
        Size = new DemonicsVector2((DemonicsFloat)size.x, (DemonicsFloat)size.y);
        Offset = new DemonicsVector2((DemonicsFloat)offset.x, (DemonicsFloat)offset.y);
    }

    protected override void EnterCollision(DemonicsCollider collider)
    {
        if (!HitConfirm)
        {
            if (collider.TryGetComponent(out Hurtbox hurtbox))
            {
                base.EnterCollision(collider);
                OnPlayerCollision?.Invoke();
                _hitboxResponder.HitboxCollided(new Vector2((float)HitPoint.x, (float)HitPoint.y), hurtbox);
                HitConfirm = true;
            }
        }
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        HitConfirm = false;
    }
}