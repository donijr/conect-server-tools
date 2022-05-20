#!/bin/bash

# Ordem campos dos dados
#identificador,ip,cliente,descricao,servidor,tipoAmbiente,informacaoExtra,usuario,nomeKey,provedorCloud


#######################################
### Declaracao de Variaveis Globais ###
#######################################

caminho_script_cs=$(realpath $0)
pasta_raiz=$(dirname ${caminho_script_cs})
#pasta_raiz="/opt/conect-server-tools"
source ${pasta_raiz}/lib/funcoes_cs

padrao_pasta_local_arquivos_baixados="${HOME}/cs_arquivos_baixados"

vetor_parametro_arquivo_configuracao=("pasta_servidor_transferencia_arquivos"
                                       "pasta_local_arquivos_baixados"
                                       "pasta_keys"
                                       "arquivo_dados"
                                      )
###########################################################################################
#### PROGRAMA ####
##################

#Verificando existência de arquivo de configuração cs.conf
if [[  -e "${pasta_raiz}/config/cs.conf" ]]; then
    # Incluindo variaveis de configuração
    source ${pasta_raiz}/config/cs.conf 
elif [[ ! -e "${pasta_raiz}/config/cs.conf" ]]; then
    # Se arquivo não existir então cria 
    echo "Arquivo de configuração não existe. Crianado arquivo ${pasta_raiz}/config/cs.conf"
    sudo bash -c ">'${pasta_raiz}/config/c s.conf'"
    sudo bash -c "echo '${vetor_parametro_arquivo_configuracao[0]}=\"\"' >> '${pasta_raiz}/config/cs.conf'"
    sudo bash -c "echo '${vetor_parametro_arquivo_configuracao[1]}=\"\"' >> '${pasta_raiz}/config/cs.conf'"
    sudo bash -c "echo '${vetor_parametro_arquivo_configuracao[2]}=\"\"' >> '${pasta_raiz}/config/cs.conf'"
    sudo bash -c "echo '${vetor_parametro_arquivo_configuracao[3]}=\"\"' >> '${pasta_raiz}/config/cs.conf'"
    echo "Arquivo ${pasta_raiz}/config/cs.conf criado"
else
    echo "Arquivo de configuração cs.conf não existe."
    echo "Por favor, criar o arquivo de configuração em /opt/conect-server-tools/config/"
    exit 1
fi
  
# Atribuindo valores padrão para vairiáveis de configuração se forem vazias ou nulas
[[ ${pasta_servidor_transferencia_arquivos:="${USER}"} ]]
[[ ${pasta_local_arquivos_baixados:="${padrao_pasta_local_arquivos_baixados}"} ]] #sugestao de caminho pra pasta 
[[ ${pasta_keys:="${HOME}/.ssh"} ]]
[[ ${arquivo_dados:="${pasta_raiz}/dados/cs_dados_servidores.csv"} ]]

pasta_app="${pasta_raiz}/app"
pasta_dados="${pasta_raiz}/dados"
pasta_config="${pasta_raiz}/config"
pasta_saida="${pasta_raiz}/saida"
valor_data=$(date +%d-%m-%y)

erro_saida_mkdir=""

# Criando pasta de destino para arquivos baixados
if [[ "${pasta_local_arquivos_baixados}" == "${padrao_pasta_local_arquivos_baixados}" ]]; then
    mkdir -p "${padrao_pasta_local_arquivos_baixados}"
    erro_saida_mkdir=$(echo $?)
    if [[ "${erro_saida_mkdir}" -eq "1" ]]; then
        echo "Não foi possível criar pasta ${padrao_pasta_local_arquivos_baixados}"
        exit 1
    fi 
fi

# Verificação de pasta local  onde os arquivos são baixados 
if [[ ! -d "${pasta_local_arquivos_baixados}" ]]; then
    echo "Pasta local do destino dos arquivos baixados não existe."
    echo "Por favor, criar a pasta ${pasta_local_arquivos_baixados} ou modificar o valor de pasta_local_arquivos_baixados no arquivo de configuração em /opt/conect-server-tools/config/cs.conf"
    exit 1
fi 

#Verificando se arquivo de dados existe
if [[ ! -e "${arquivo_dados}" ]]; then
    echo "Arquivo de dados dos servidores não existe"
    echo "Por favor,  modificar o valor de arquivo_dados no arquivo de configuração em /opt/conect-server-tools/config/cs.conf"
    exit 1
fi 

# verificando se pasta de keys existe
if [[ ! -d ${pasta_keys} ]]; then
    echo "Pasta das keys não existe."
    echo "Por favor,  criar a pasta ${pasta_keys} ou modificar o valor de pasta_keys no arquivo de configuração em /opt/conect-server-tools/config/cs.conf"
    exit 1
fi 

OPTIND=1
while getopts ":c:e:b:i:sdhC" OPCAO
do
    export padrao_servidor=$OPTARG

    case $OPCAO in
        c)
            if [ "$(retornar_estado_padrao_servidor ${padrao_servidor})" == "1" ]; then
                echo "Servidor $OPTARG não encontrado"
            else
                conectar_servidor "${padrao_servidor}" "ssh"
                
            fi
            ;;
        e) 
            if [ "$(retornar_estado_padrao_servidor ${padrao_servidor})" == "1" ]; then
                echo "Servidor $OPTARG não encontrado"
            else
                conectar_servidor "${padrao_servidor}" "l2r"
            fi
            ;;
        b) 
            if [ "$(retornar_estado_padrao_servidor ${padrao_servidor})" == "1" ]; then
               echo "Servidor $OPTARG não encontrado"
            else
                conectar_servidor "${padrao_servidor}" "r2l"
            fi
            ;;
        i)
            if [ "$(retornar_estado_padrao_servidor ${padrao_servidor})" == "1" ]; then
                echo "Servidor $OPTARG não encontrado"
            else
                exibir_informacao_servidor "${padrao_servidor}"
            fi
            ;;
        s)
            cat ${arquivo_dados}
            ;;
        d) 
            echo ${arquivo_dados}
            ;;
        C)
            exibir_configuracao
            ;;
        h)
            exibir_uso
            ;;
        \?) 
            echo "Opção $OPTARG Inválida !"
            exit 1
            ;;
        :) 
            echo "Opção $OPTARG precisa de um parâmetro"
            exit 1
            ;;
    esac
done

shift $((--OPTIND))