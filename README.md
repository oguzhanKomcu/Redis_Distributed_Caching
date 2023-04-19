# Redis_Distributed_Caching

Docker da conteiner işlemi için şunları yapmak gerekir.
 -- Azure.Containers.Tools.Target nuget paketi projenin web tarafına kurulur.
 -- Kuurlduktan sonra yeni item eklemek için Add denir. Sonrasında docker file oluşturmak için DockerSupport çalıştırılır ve projenin dockerfile dosyası hazırlanır.
 -- Sonrasında yine Add-Docker Orcestrator Subbort   Docker Compose seçili olarak çalıştırılır.
 -- Sonrasında proje içinde olcuak şekilde terminal açılır. Bu arada dockerında açık olamsı gerekir. 
 -- Terminale " docker build -f Redis_Distributed_Caching/Dockerfile -t testdocker:v1 . " bu sekilde bilgiler verilir ve başlatılır.
 -- Sonrasında containerın çalışır hale gelmesi için " docker run -it --rm -p 8080:80 testdocker:v1 " bu şekilde host numarası verilir ve çalıştırılır.
