USE EvaluacionTecnicaDB;
GO

-- â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
-- SEED: 6 Evaluaciones adicionales de prueba
-- Tecnologias: TypeScript/Node, Microservicios, DevOps,
--              Java Spring Boot, Vue.js 3, Algoritmos
-- â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

-- â”€â”€â”€ Evaluaciones â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO Evaluaciones (Id, Titulo, Descripcion, Tecnologia, Nivel, Estado, TiempoLimiteTotalMinutos, RequiereCamara, RequiereMicrofono, EvaluadorId, CreadoEn, EstaActivo) VALUES
('E2000000-0000-0000-0000-000000000001', N'Evaluacion TypeScript y Node.js',               N'Prueba para desarrolladores backend en TypeScript con Node.js. Cubre tipado avanzado, genericos, async/await y desarrollo de APIs REST con Express.',                                                              N'TypeScript / Node.js',           2, 2, 60,  0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-15 09:00:00', 1),
('E2000000-0000-0000-0000-000000000002', N'Evaluacion Arquitectura de Microservicios',      N'Prueba senior sobre diseno de sistemas distribuidos, patrones como Saga, CQRS, Event Sourcing, Circuit Breaker y comunicacion entre servicios.',                                                                   N'Arquitectura / Microservicios',  3, 2, 90,  0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-16 09:00:00', 1),
('E2000000-0000-0000-0000-000000000003', N'Evaluacion DevOps y Contenedores',               N'Evaluacion para ingenieros DevOps: Docker, Kubernetes, CI/CD, infraestructura como codigo y practicas de despliegue continuo.',                                                                                    N'DevOps / Docker / Kubernetes',   2, 2, 75,  0, 0, 'B1000000-0000-0000-0000-000000000002', '2026-03-16 11:00:00', 1),
('E2000000-0000-0000-0000-000000000004', N'Evaluacion Java Spring Boot',                    N'Prueba tecnica para desarrolladores Java senior. Cubre Spring Boot, Spring Security, JPA, AOP, manejo de transacciones y patrones de diseno empresariales.',                                                       N'Java / Spring Boot',             3, 1, 90,  0, 0, 'B1000000-0000-0000-0000-000000000002', '2026-03-17 09:00:00', 1),
('E2000000-0000-0000-0000-000000000005', N'Evaluacion Vue.js 3 Fundamentals',               N'Evaluacion para desarrolladores junior-mid en Vue.js 3: Composition API, ref, reactive, componentes, props, emits, watchers y ciclo de vida.',                                                                    N'Vue.js',                         1, 2, 45,  0, 0, 'B1000000-0000-0000-0000-000000000001', '2026-03-17 14:00:00', 1),
('E2000000-0000-0000-0000-000000000006', N'Evaluacion Algoritmos y Estructuras de Datos',   N'Prueba avanzada de algoritmia: complejidad temporal y espacial, estructuras de datos clasicas, algoritmos de ordenamiento, busqueda binaria, heaps y grafos.',                                                    N'Algoritmos / CS Fundamentals',   4, 2, 120, 0, 0, 'B1000000-0000-0000-0000-000000000002', '2026-03-18 09:00:00', 1);
GO

-- â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
-- PREGUNTAS
-- â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

-- â”€â”€â”€ Eval 1: TypeScript / Node.js (5 preguntas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('F2000000-0000-0000-0000-000000000001',
 N'Explica la diferencia entre interface y type en TypeScript. Da un ejemplo de cuando usarias cada uno y menciona al menos dos diferencias clave entre ambos.',
 1, N'Debe mencionar: interface soporta declaration merging y herencia con extends, type permite uniones/intersecciones y alias de primitivos. Con ejemplos concretos de cada caso de uso.', 20, 1, 480, 'E2000000-0000-0000-0000-000000000001', '2026-03-15 09:10:00', 1),

('F2000000-0000-0000-0000-000000000002',
 N'Implementa en TypeScript una funcion generica fetchData<T> que realice una peticion HTTP GET, maneje errores de red y status HTTP no exitosos, y retorne el dato correctamente tipado.',
 2, N'Debe usar genericos <T>, async/await, try/catch, verificar response.ok, lanzar error con status, retornar Promise<T>. Sin uso de any. Tipos explÃ­citos en firma y retorno.', 30, 2, 720, 'E2000000-0000-0000-0000-000000000001', '2026-03-15 09:15:00', 1),

('F2000000-0000-0000-0000-000000000003',
 N'Explica el Event Loop de Node.js. Describe las fases del ciclo: timers, I/O callbacks, poll, check y close. Cual es la diferencia entre process.nextTick() y setImmediate()?',
 1, N'Debe describir las fases del Event Loop, mencionar la call stack y microtask queue. nextTick: se ejecuta antes del siguiente ciclo del event loop (despues del codigo actual). setImmediate: se ejecuta en la fase check del proximo ciclo.', 25, 3, 600, 'E2000000-0000-0000-0000-000000000001', '2026-03-15 09:20:00', 1),

('F2000000-0000-0000-0000-000000000004',
 N'Cual de las siguientes afirmaciones sobre los decoradores en TypeScript es CORRECTA?',
 3, N'Respuesta correcta: Los decoradores son funciones que se ejecutan en tiempo de definicion de la clase y pueden modificar o anotar el comportamiento de clases, metodos, propiedades y parametros.', 15, 4, 240, 'E2000000-0000-0000-0000-000000000001', '2026-03-15 09:25:00', 1),

('F2000000-0000-0000-0000-000000000005',
 N'Implementa un middleware de Express en TypeScript que autentique requests usando JWT. Debe extraer el token del header Authorization (Bearer), verificarlo y agregar el payload decodificado al objeto Request.',
 2, N'Debe extender la interfaz Request con el usuario tipado, extraer Bearer token, verificar con jsonwebtoken, crear tipo para payload, manejar errores 401 para token invalido y ausente, llamar next() si valido.', 25, 5, 720, 'E2000000-0000-0000-0000-000000000001', '2026-03-15 09:30:00', 1);
GO

-- â”€â”€â”€ Eval 2: Arquitectura de Microservicios (5 preguntas) â”€â”€â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('F3000000-0000-0000-0000-000000000001',
 N'Explica el patron Saga para manejo de transacciones distribuidas. Describe las dos variantes: coreografia y orquestacion, y cuando elegirias una sobre la otra.',
 1, N'Debe explicar que Saga divide la transaccion en pasos locales con transacciones de compensacion. Coreografia: servicios reaccionan a eventos sin coordinador. Orquestacion: un Saga Orchestrator coordina los pasos. Tradeoffs de cada enfoque.', 25, 1, 720, 'E2000000-0000-0000-0000-000000000002', '2026-03-16 09:10:00', 1),

('F3000000-0000-0000-0000-000000000002',
 N'Que es CQRS (Command Query Responsibility Segregation)? Como se combina con Event Sourcing? Describe un escenario real donde implementarias ambos patrones juntos.',
 1, N'CQRS: separar modelos de escritura (commands) y lectura (queries). Event Sourcing: guardar eventos en vez de estado actual. Combinados: los comandos generan eventos que se proyectan en read models. Escalabilidad y auditoria.', 25, 2, 720, 'E2000000-0000-0000-0000-000000000002', '2026-03-16 09:15:00', 1),

('F3000000-0000-0000-0000-000000000003',
 N'Implementa en el lenguaje de tu eleccion un Circuit Breaker con tres estados: Closed, Open y Half-Open. Define las transiciones entre estados, el umbral de fallos y el timeout de recuperacion.',
 2, N'Debe implementar los 3 estados, contador de fallos, umbral configurable (ej: 5 fallos), timeout para transicion a Half-Open, logica de transicion entre estados. Puede ser clase, funcion wrapper o patron decorator.', 30, 3, 900, 'E2000000-0000-0000-0000-000000000002', '2026-03-16 09:20:00', 1),

('F3000000-0000-0000-0000-000000000004',
 N'Cual de los siguientes es considerado un ANTIPATRON en arquitecturas de microservicios?',
 3, N'Respuesta correcta: Shared Database es un antipatron ya que crea acoplamiento entre servicios. Cada microservicio debe ser dueno exclusivo de su propia base de datos. REST, gRPC y Message Queue son patrones de comunicacion validos.', 15, 4, 300, 'E2000000-0000-0000-0000-000000000002', '2026-03-16 09:25:00', 1),

('F3000000-0000-0000-0000-000000000005',
 N'Describe como implementarias service discovery en una arquitectura de microservicios. Explica la diferencia entre client-side y server-side discovery con ejemplos de herramientas en cada caso.',
 1, N'Client-side: cliente consulta el service registry (Eureka, Consul) y elige la instancia. Server-side: el load balancer consulta el registry (AWS ALB, Nginx + Consul). Trade-offs: client-side da mas control, server-side simplifica el cliente.', 20, 5, 600, 'E2000000-0000-0000-0000-000000000002', '2026-03-16 09:30:00', 1);
GO

-- â”€â”€â”€ Eval 3: DevOps y Contenedores (5 preguntas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('F4000000-0000-0000-0000-000000000001',
 N'Explica la diferencia entre un contenedor Docker y una maquina virtual. Que es una imagen Docker, que son los layers y como funciona el union filesystem?',
 1, N'VM: virtualiza hardware completo con SO propio y hypervisor. Contenedor: comparte el kernel del host, aislado por namespaces y cgroups. Imagen: capas inmutables apiladas (union filesystem). Cada instruccion Dockerfile crea un layer de solo lectura.', 20, 1, 480, 'E2000000-0000-0000-0000-000000000003', '2026-03-16 11:10:00', 1),

('F4000000-0000-0000-0000-000000000002',
 N'Escribe un Dockerfile multi-stage optimizado para una aplicacion Node.js de produccion. La imagen final debe ser minima y solo contener los artefactos necesarios para ejecutar la app.',
 2, N'Debe usar FROM node:alpine AS builder para compilar, segunda etapa FROM node:alpine con solo node_modules de produccion y carpeta de build. User no-root, COPY --from correctamente, CMD con node directamente. Sin devDependencies en la imagen final.', 30, 2, 720, 'E2000000-0000-0000-0000-000000000003', '2026-03-16 11:15:00', 1),

('F4000000-0000-0000-0000-000000000003',
 N'Cual es la diferencia principal entre un Deployment y un StatefulSet en Kubernetes? Da un ejemplo de cuando usarias cada uno.',
 3, N'Deployment: pods sin identidad estable, intercambiables, ideales para apps sin estado (APIs, frontends). StatefulSet: identidad estable (nombre-0, nombre-1), almacenamiento persistente por pod, orden garantizado. Ideal para bases de datos, Kafka, ZooKeeper.', 15, 3, 300, 'E2000000-0000-0000-0000-000000000003', '2026-03-16 11:20:00', 1),

('F4000000-0000-0000-0000-000000000004',
 N'Escribe un pipeline de CI/CD en GitHub Actions para una app Node.js que: ejecute los tests, construya y publique la imagen Docker, y haga deploy actualizado en un cluster Kubernetes.',
 2, N'Debe incluir: trigger on push a main, jobs separados (test, build-push, deploy), secrets para registry y kubeconfig, docker build con tag por SHA de commit, kubectl set image o helm upgrade en el job de deploy.', 30, 4, 900, 'E2000000-0000-0000-0000-000000000003', '2026-03-16 11:25:00', 1),

('F4000000-0000-0000-0000-000000000005',
 N'Que es Infrastructure as Code (IaC)? Explica la diferencia entre Terraform y Ansible y describe un escenario donde usarias ambas herramientas de forma complementaria.',
 1, N'IaC: definir infraestructura en archivos de codigo versionables y reproducibles. Terraform: provision de infraestructura (declarativo, cloud-agnostic, state). Ansible: configuracion y gestion de servidores (idempotente). Flujo: Terraform crea VMs, Ansible instala y configura el software.', 20, 5, 600, 'E2000000-0000-0000-0000-000000000003', '2026-03-16 11:30:00', 1);
GO

-- â”€â”€â”€ Eval 4: Java Spring Boot (5 preguntas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('F5000000-0000-0000-0000-000000000001',
 N'Que es Spring AOP? Explica los conceptos de Aspect, Advice, Pointcut y Join Point con sus relaciones. Da un ejemplo practico de cuado lo usarias en una aplicacion real.',
 1, N'AOP separa cross-cutting concerns. Aspect: modulo que encapsula la logica transversal. Advice: accion en un join point (Before/After/Around/AfterThrowing). Pointcut: expresion que selecciona join points. Ejemplos practicos: logging, auditorÃ­a, seguridad, transacciones.', 20, 1, 600, 'E2000000-0000-0000-0000-000000000004', '2026-03-17 09:10:00', 1),

('F5000000-0000-0000-0000-000000000002',
 N'Implementa un endpoint GET /api/productos con paginacion en Spring Boot y Spring Data JPA. Debe aceptar parametros page, size y sort, y retornar una respuesta paginada con metadatos.',
 2, N'Debe usar @GetMapping, Pageable como parametro (resuelto automaticamente por Spring), Page<Producto> del JpaRepository, mapeo a DTO con stream().map(), ResponseEntity<Page<ProductoDto>>. Puede incluir @PageableDefault para valores por defecto.', 30, 2, 900, 'E2000000-0000-0000-0000-000000000004', '2026-03-17 09:15:00', 1),

('F5000000-0000-0000-0000-000000000003',
 N'Cual es la diferencia entre @Transactional(propagation = REQUIRES_NEW) y @Transactional(propagation = NESTED) en Spring?',
 3, N'REQUIRES_NEW: suspende la transaccion externa y crea una nueva completamente independiente. El rollback de la interna no afecta a la externa. NESTED: crea un savepoint dentro de la transaccion externa. El rollback de la interna solo retrocede al savepoint. NESTED requiere soporte del driver JDBC.', 20, 3, 360, 'E2000000-0000-0000-0000-000000000004', '2026-03-17 09:20:00', 1),

('F5000000-0000-0000-0000-000000000004',
 N'Explica la diferencia entre @Component, @Service, @Repository y @Controller en Spring. Son intercambiables tecnicamente? Por que se recomienda usar el correcto en cada capa?',
 1, N'Todos son especializaciones de @Component. @Repository habilita traduccion de excepciones de persistencia. @Service: semantica de capa de negocio. @Controller/@RestController: procesamiento MVC/REST. No son 100% intercambiables (@Repository tiene comportamiento extra). La especializacion da claridad arquitectural y facilita scanning selectivo.', 20, 4, 480, 'E2000000-0000-0000-0000-000000000004', '2026-03-17 09:25:00', 1),

('F5000000-0000-0000-0000-000000000005',
 N'Implementa un filtro de seguridad personalizado en Spring Security que valide un API Key en el header X-API-Key. Si es valido establece el SecurityContext; si no, retorna HTTP 401.',
 2, N'Debe extender OncePerRequestFilter, leer header X-API-Key, comparar con valor configurado (inyectado por @Value), crear UsernamePasswordAuthenticationToken si es valido y setear en SecurityContextHolder. En caso de fallo: response.setStatus(401) y return sin llamar filterChain.', 25, 5, 720, 'E2000000-0000-0000-0000-000000000004', '2026-03-17 09:30:00', 1);
GO

-- â”€â”€â”€ Eval 5: Vue.js 3 (4 preguntas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('F6000000-0000-0000-0000-000000000001',
 N'Explica la diferencia entre ref() y reactive() en Vue 3 Composition API. Cuando usarias cada uno? Que es .value y cuando es necesario acceder a el?',
 1, N'ref(): para primitivos y objetos, requiere .value en JS (no en template, Vue lo desenvuelve). reactive(): solo objetos, reactivo en profundidad, sin .value, pierde reactividad si se desestructura. Regla practica: ref para primitivos simples, reactive para objetos con multiples propiedades relacionadas.', 20, 1, 480, 'E2000000-0000-0000-0000-000000000005', '2026-03-17 14:10:00', 1),

('F6000000-0000-0000-0000-000000000002',
 N'Crea un componente Vue 3 con Composition API llamado ListaBusqueda que: reciba un array de items como prop, permita filtrar en tiempo real con un input de texto y muestre los resultados filtrados.',
 2, N'Debe usar <script setup>, defineProps con tipo correcto, ref para el termino de busqueda, computed para la lista filtrada (filter con includes), v-model en el input, v-for con :key en el listado. Componente limpio sin logica innecesaria.', 30, 2, 720, 'E2000000-0000-0000-0000-000000000005', '2026-03-17 14:15:00', 1),

('F6000000-0000-0000-0000-000000000003',
 N'Cual de las siguientes opciones describe correctamente como pasar datos de un componente hijo al componente padre en Vue 3?',
 3, N'Respuesta correcta: usando defineEmits() en el componente hijo para declarar los eventos permitidos, llamando a emit(''nombre-evento'', payload) para dispararlos, y escuchando con @nombre-evento en el padre. Props fluyen hacia abajo, eventos hacia arriba.', 15, 3, 300, 'E2000000-0000-0000-0000-000000000005', '2026-03-17 14:20:00', 1),

('F6000000-0000-0000-0000-000000000004',
 N'Explica los hooks del ciclo de vida de Vue 3: onMounted, onUpdated y onUnmounted. Cuando usarias cada uno? Da un ejemplo practico de un caso de uso real para cada hook.',
 1, N'onMounted: DOM disponible, ideal para llamadas a API, inicializar librerias de terceros que necesitan el DOM. onUpdated: DOM actualizado post-render, cuidado con loops infinitos. onUnmounted: cleanup de event listeners, timers (clearInterval), suscripciones a stores o WebSockets para evitar memory leaks.', 20, 4, 480, 'E2000000-0000-0000-0000-000000000005', '2026-03-17 14:25:00', 1);
GO

-- â”€â”€â”€ Eval 6: Algoritmos y Estructuras de Datos (5 preguntas) â”€
INSERT INTO Preguntas (Id, Texto, Tipo, Rubrica, PuntajeMaximo, OrdenEnEvaluacion, TiempoLimiteSegundos, EvaluacionId, CreadoEn, EstaActivo) VALUES
('FA000000-0000-0000-0000-000000000001',
 N'Explica la notacion Big O. Describe y compara la complejidad de Bubble Sort, Merge Sort y Quick Sort en caso promedio y peor caso. Por que Quick Sort suele tener mejor rendimiento en la practica que Merge Sort?',
 1, N'Big O: limite superior del crecimiento. Bubble O(n2) ambos casos. Merge O(n log n) siempre pero O(n) espacio extra. Quick O(n log n) promedio, O(n2) peor caso (pivote malo) pero O(log n) espacio. Quick mas rapido en la practica por menor constante y mejor cache locality (in-place).', 25, 1, 720, 'E2000000-0000-0000-0000-000000000006', '2026-03-18 09:10:00', 1),

('FA000000-0000-0000-0000-000000000002',
 N'Implementa el algoritmo de busqueda binaria en sus dos versiones: iterativa y recursiva. Analiza y compara la complejidad temporal y espacial de cada version.',
 2, N'Iterativa: O(log n) tiempo, O(1) espacio (sin stack). Recursiva: O(log n) tiempo, O(log n) espacio por call stack. Ambas deben retornar el indice del elemento o -1 si no existe. Manejo correcto de los limites (low, high, mid = low + (high-low)/2 para evitar overflow).', 25, 2, 720, 'E2000000-0000-0000-0000-000000000006', '2026-03-18 09:15:00', 1),

('FA000000-0000-0000-0000-000000000003',
 N'Cual combinacion de estructuras de datos usarias para implementar una cache LRU (Least Recently Used) con operaciones get y put en O(1) garantizado?',
 3, N'Respuesta correcta: HashMap + Lista doblemente enlazada. El HashMap provee acceso O(1) a cualquier nodo. La lista doblemente enlazada mantiene el orden de uso (head = mas reciente, tail = menos reciente) y permite mover y eliminar nodos en O(1) sin necesidad de busqueda.', 20, 3, 360, 'E2000000-0000-0000-0000-000000000006', '2026-03-18 09:20:00', 1),

('FA000000-0000-0000-0000-000000000004',
 N'Implementa una clase MinHeap con los metodos insert(value), extractMin() y peek(). Explica como se mantiene la propiedad heap en cada operacion describiendo heapify-up y heapify-down.',
 2, N'Array interno donde parent(i) = (i-1)/2, left(i) = 2i+1, right(i) = 2i+2. insert: agregar al final y heapify-up (comparar con padre, intercambiar si menor, repetir). extractMin: guardar raiz, mover ultimo a raiz, heapify-down (intercambiar con el hijo menor hasta restaurar heap). Ambas O(log n).', 30, 4, 1200, 'E2000000-0000-0000-0000-000000000006', '2026-03-18 09:25:00', 1),

('FA000000-0000-0000-0000-000000000005',
 N'Implementa el algoritmo de Dijkstra para encontrar el camino mas corto desde un nodo origen a todos los demas en un grafo dirigido ponderado. Explica por que falla con aristas de peso negativo.',
 2, N'Debe usar min-heap/priority queue con (distancia, nodo), array de distancias inicializado a infinito, relajacion de aristas vecinas, set de visitados. Falla con pesos negativos porque asume que al extraer un nodo del heap su distancia es definitiva (greedy). Pesos negativos pueden invalidar esa asuncion. Para pesos negativos usar Bellman-Ford.', 35, 5, 1500, 'E2000000-0000-0000-0000-000000000006', '2026-03-18 09:30:00', 1);
GO

-- â”€â”€â”€ Verificacion â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
SELECT
    e.Titulo,
    e.Tecnologia,
    CASE e.Nivel WHEN 1 THEN 'Junior' WHEN 2 THEN 'Mid' WHEN 3 THEN 'Senior' WHEN 4 THEN 'Lead' END AS Nivel,
    COUNT(p.Id) AS TotalPreguntas,
    SUM(p.PuntajeMaximo) AS PuntajeTotal
FROM Evaluaciones e
LEFT JOIN Preguntas p ON p.EvaluacionId = e.Id AND p.EstaActivo = 1
WHERE e.Id LIKE 'E2%'
GROUP BY e.Id, e.Titulo, e.Tecnologia, e.Nivel
ORDER BY e.CreadoEn;
GO
