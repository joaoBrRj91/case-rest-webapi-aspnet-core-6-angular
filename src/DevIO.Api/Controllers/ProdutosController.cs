using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoService produtoService;
        private readonly IMapper mapper;

        public ProdutosController(IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            this.produtoRepository = produtoRepository;
            this.produtoService = produtoService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> ObterTodos()
        {
            var produtosViewModel =  mapper.Map<IEnumerable<ProdutoViewModel>>(await produtoRepository.ObterProdutosFornecedores());

            return CustomResponse(produtosViewModel);
        }

        [HttpGet("id:guid")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = mapper.Map<ProdutoViewModel>(await produtoRepository.ObterPorId(id));

            if (produtoViewModel is null) return NotFound();

            return CustomResponse(produtoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            var imagemNome = $"{produtoViewModel.Imagem}_{Guid.NewGuid()}";

            var uploadArquivoSucesso =
                UploadArquivo(produtoViewModel.ImagemUpload, imagemNome);

            if (uploadArquivoSucesso)
                await produtoService.Adicionar(mapper.Map<Produto>(produtoViewModel));

            produtoViewModel.Imagem = imagemNome;
            return CustomResponse(produtoViewModel);
        }

        [HttpDelete("id:guid")]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> Remover(Guid id)
        {
            var produtoViewModel = mapper.Map<ProdutoViewModel>(await produtoRepository.ObterPorId(id));

            if (produtoViewModel is null) return NotFound();

            await produtoRepository.Remover(id);

            return Ok(produtoViewModel);
        }


        #region Metodos Privados
        private bool UploadArquivo(string arquivo64, string imgNome)
        {
            if(string.IsNullOrEmpty(arquivo64) || string.IsNullOrEmpty(imgNome))
            {
                NotificarErro("Forneça uma imagem e o nome dela para esse produto!");
                return false;
            }

            var imagemDataByteArray = Convert.FromBase64String(arquivo64);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com esse nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imagemDataByteArray);
            return true;

        }
        #endregion

    }
}

