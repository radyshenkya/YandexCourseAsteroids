using UnityEngine;
using System.Collections;
using Asteroids.Model;
using UnityEngine.SceneManagement;

namespace CompositeRoot
{
    public class PhysicsRoutingCompositeRoot : CompositeRoot
    {
        [SerializeField] private EnemiesCompositeRoot _enemiesRoot;
        [SerializeField] private ShipCompositeRoot _shipRoot;
        [SerializeField] private EndGameWindowView _endGameWindow;
        [SerializeField] private PhysicsEventsBroadcaster _shipEventsBroadcaster;

        private PhysicsRouter _router;
        private CollisionsRecords _records;

        public PhysicsRouter Model => _router;

        public override void Compose()
        {
            _records = new CollisionsRecords(_shipRoot.Bullets, _enemiesRoot.Simulation);
            _router = new PhysicsRouter(_records.Values);
            _shipEventsBroadcaster.Init(_router, _shipRoot.Model);

            StartCoroutine(GetRouterSteper());
        }

        private void OnEnable()
        {
            _records.ShipDamage += OnShipDamage;
        }

        private void OnDisable()
        {
            _records.ShipDamage -= OnShipDamage;
        }

        private void OnShipDamage()
        {
            _shipRoot.DecreaseShipHealth();

            if (_shipRoot.Health <= 0)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            _shipRoot.DisableShip();

            _endGameWindow.Show(0, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }

        private IEnumerator GetRouterSteper()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                _router.Step();
            }
        }
    }
}
